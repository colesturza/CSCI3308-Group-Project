using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security.Authentication.Interfaces;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Entities.Users.Management;

namespace UHub.CoreLib.Security.Authentication
{

    internal sealed partial class AuthenticationManager : IAuthenticationManager
    {
        /// <summary>
        /// Request variable to store the current user.  Used as temp caching to prevent extra lookups
        /// </summary>
        internal const string REQUEST_CURRENT_USER = "Global_CurrentUser";


        private AuthenticationProvider authWorker;
        internal AuthenticationManager()
        {
            authWorker = new FormsAuthProvider();
        }


        /// <summary>
        /// Validate user credentials then set authentication token via cookie
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <param name="IsPersistent">Flag to set token persistence status</param>
        /// <paramref name="ResultCode">Result code to indicate process status</paramref>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        public AuthResultCode TrySetClientAuthToken(
            string UserEmail,
            string UserPassword,
            bool IsPersistent,
            Action<Guid> GeneralFailHandler = null)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            bool userTokenHandler(User cmsUser)
            {
                var authCookieName = CoreFactory.Singleton.Properties.AuthTknCookieName;

                //remove old token
                HttpContext.Current.Request.Cookies.Remove(authCookieName);
                HttpContext.Current.Response.Cookies.Remove(authCookieName);

                //everything good
                //write user auth cookie
                try
                {
                    authWorker.SetCurrentUser_Local(cmsUser);
                    authWorker.SetCurrentUser_ClientToken(IsPersistent, cmsUser);
                    return true;
                }
                catch
                {

                    GeneralFailHandler(new Guid("070E2C1C-9162-4175-AAB8-018BB38F095D"));
                    return false;
                }
            }


            return authWorker.TryAuthenticateUser(
                UserEmail,
                UserPassword,
                GeneralFailHandler,
                userTokenHandler);

        }

        /// <summary>
        /// Validate user credentials then return encrypted authentication token
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <param name="IsPersistent">Flag to set token persistence status</param>
        /// <paramref name="ResultCode">Result code to indicate process status</paramref>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        public string TryGetClientAuthToken(
            string UserEmail,
            string UserPassword,
            bool IsPersistent,
            out AuthResultCode ResultCode,
            Action<Guid> GeneralFailHandler = null)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            string token = "";

            bool userTokenHandler(User cmsUser)
            {
                //everything good
                //write user auth cookie
                try
                {
                    var context = HttpContext.Current;
                    var tkn = authWorker.GenerateAuthToken(IsPersistent, cmsUser, context);
                    token = tkn.Encrypt();
                    return true;
                }
                catch (CryptographicException ex)
                {
                    token = "ERROR";
                    GeneralFailHandler(new Guid("B51AD0A9-1E02-4911-AA67-FC60A0E19E90"));
                    CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex);

                    return false;
                }
                catch (Exception ex)
                {
                    token = "ERROR";
                    GeneralFailHandler(new Guid("88CB24F0-139D-42BE-82D5-56666580323D"));
                    CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex);

                    return false;
                }
            }


            ResultCode = authWorker.TryAuthenticateUser(
                UserEmail,
                UserPassword,
                GeneralFailHandler,
                userTokenHandler);


            return token;
        }


        /// <summary>
        /// Slide the expiration date of a token and return a new encrypted client token
        /// <para/> If token cannot be extend, then the original token is returned
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string TrySlideAuthTokenExpiration(string token) => TrySlideAuthTokenExpiration(token, out _);

        /// <summary>
        /// Slide the expiration date of a token and return a new encrypted client token
        /// <para/> If token cannot be extend, then the original token is returned
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string TrySlideAuthTokenExpiration(string token, out TokenValidationStatus TokenStatus)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            string newToken = token;

            void succHandler(AuthenticationToken authToken)
            {
                authWorker.SlideAuthTokenExpiration(authToken);
                newToken = authToken.Encrypt();
            }

            TokenStatus = authWorker.ValidateAuthToken(token, out _, () => { TryLogOut(5); }, succHandler);

            return newToken;
        }


        /// <summary>
        /// Try to authenticate a user account using the supplied account credentials
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <paramref name="ResultCode">Result code to indicate process status</paramref>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        public AuthResultCode TryAuthenticateUser(
            string UserEmail,
            string UserPassword,
            Action<Guid> GeneralFailHandler = null)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            return authWorker.TryAuthenticateUser(
                UserEmail,
                UserPassword,
                GeneralFailHandler,
                null);
        }


        /// <summary>
        /// Get the url that the user should be redirected to after login
        /// </summary>
        public string GetAuthForwardUrl()
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            var postAuthCookieName = CoreFactory.Singleton.Properties.PostAuthCookieName;
            var cookie = HttpContext.Current.Request.Cookies[postAuthCookieName];


            var loginUrl = CoreFactory.Singleton.Properties.LoginURL;
            var defaultFwdUrl = CoreFactory.Singleton.Properties.DefaultAuthFwdURL;


            if (cookie != null)
            {
                cookie.Decrypt();
                if (cookie.Value.IsNotEmpty() && cookie.Value != loginUrl)
                {
                    //go to requested page

                    if (Uri.TryCreate(cookie.Value, UriKind.Absolute, out var url))
                    {
                        return url.AbsolutePath;
                    }

                    return defaultFwdUrl;
                }
                else
                {
                    //go to default page
                    return defaultFwdUrl;
                }
            }
            else
            {
                return defaultFwdUrl;
            }
        }

        /// <summary>
        /// Forward user to originally requested page (if set) or default auth page
        /// </summary>
        public void AuthUserPageForward()
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            //forward to originally requested page
            var url = GetAuthForwardUrl();

            HttpContext.Current.Response.Redirect(url);
        }

        /// <summary>
        /// Set the user for the current request using an Auth Token.  If the token is invalid, then the user will be set to ANON privs.
        /// This method is provided so that restful API requests without an auth cookie can be authenticated throughout the CMS pipeline
        /// </summary>
        /// <param name="tokenStr">AuthToken in string form</param>
        /// <param name="tokenStatus">Returns token validation status</param>
        /// <returns>Status flag</returns>
        public TokenValidationStatus TrySetRequestUser(string tokenStr)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            var tokenStatus = ValidateAuthToken(tokenStr, out User cmsUser);
            if (tokenStatus != TokenValidationStatus.Success)
            {
                cmsUser = UserReader.GetAnonymousUser();
                HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER] = cmsUser;
                return tokenStatus;
            }


            if (cmsUser == null || cmsUser.ID == null)
            {
                cmsUser = UserReader.GetAnonymousUser();
                HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER] = cmsUser;
                return TokenValidationStatus.AnonUser;
            }


            HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER] = cmsUser;

            return TokenValidationStatus.Success;
        }

        /// <summary>
        /// Ensure that auth token is valid and user is logged in
        /// </summary>
        /// <param name="tokenStr">Auth token in string form</param>
        /// <param name="CmsUser">User encapsulated by auth token (if valid)</param>
        /// <param name="tokenStatus">Return auth token validation status</param>
        /// <returns></returns>
        public TokenValidationStatus ValidateAuthToken(string tokenStr, out User CmsUser)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            var tokenStatus = authWorker.ValidateAuthToken(tokenStr, out CmsUser, null);
            return tokenStatus;
        }

        /// <summary>
        /// Check if user is logged in
        /// </summary>
        /// <returns></returns>
        public bool IsUserLoggedIn()
        {
            return IsUserLoggedIn(out _, out _);
        }

        /// <summary>
        /// Check if user is logged in, and return the current user
        /// </summary>
        /// <param name="CmsUser"></param>
        /// <returns></returns>
        public bool IsUserLoggedIn(out User CmsUser)
        {
            return IsUserLoggedIn(out CmsUser, out _);
        }

        /// <summary>
        /// Determine if there is a user logged in
        /// Uses the auth cookie and various expiration timers
        /// Returns the authenticated user or a reference to Anon instance
        /// </summary>
        /// <returns></returns>
        public bool IsUserLoggedIn(out User CmsUser, out TokenValidationStatus tokenStatus)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


#if (!TESTING)
            var requestUser = HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER];

            if (false && requestUser != null && requestUser is User currentUser)
            {
                CmsUser = currentUser;
                if (currentUser.ID == null)
                {
                    tokenStatus = TokenValidationStatus.AnonUser;
                    return false;
                }
                tokenStatus = TokenValidationStatus.Success;
                return true;
            }
            else
            {
                //get user info from tkn (if it exists)
                tokenStatus = authWorker.ValidateAuthCookie(out CmsUser);

                if (CmsUser == null || CmsUser.ID == null)
                {
                    CmsUser = UserReader.GetAnonymousUser();
                    HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER] = CmsUser;
                    return false;
                }

                HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER] = CmsUser;

                //check for real user vs Anon user
                if (CmsUser.ID == null)
                {
                    if (tokenStatus == TokenValidationStatus.Success)
                    {
                        tokenStatus = TokenValidationStatus.AnonUser;
                    }
                    return false;
                }

                tokenStatus = TokenValidationStatus.Success;
                return true;
            }
#else
            cmsUser = ParentManager.UserReader.GetAnonymousUser();
            return false;
#endif
        }


        /// <summary>
        /// Get the currently authenticated CMS user. If the user is not authenticated, then an anonymous user is returned (UID=null, class=Anon)
        /// </summary>
        /// <returns></returns>
        public User GetCurrentUser()
        {
            return GetCurrentUser(out _);
        }


        /// <summary>
        /// Get the currently authenticated CMS user. If the user is not authenticated, then an anonymous user is returned (UID=null, class=Anon)
        /// </summary>
        /// <returns></returns>
        public User GetCurrentUser(out TokenValidationStatus tokenStatus)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                var requestUser = HttpContext.Current?.Items[AuthenticationManager.REQUEST_CURRENT_USER];


                if (requestUser != null && requestUser is User currentUser)
                {
                    tokenStatus = TokenValidationStatus.Success;
                    return currentUser;
                }
                else
                {
                    //get user info from tkt (if it exists)
                    tokenStatus = authWorker.ValidateAuthCookie(out User cmsUser);


                    if (cmsUser == null || cmsUser.ID == null)
                    {
                        cmsUser = UserReader.GetAnonymousUser();
                        HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER] = cmsUser;

                        return cmsUser;
                    }
                    HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER] = cmsUser;

                    //check for real user vs Anon user
                    return cmsUser;
                }

            }
            catch (Exception ex)
            {
                tokenStatus = TokenValidationStatus.AnonUser;
                return UserReader.GetAnonymousUser();
            }
        }


        /// <summary>
        /// Logout of all devices by incrementing user version
        /// </summary>
        public void LogoutOfAllDevices(long UserID, bool ExcludeCurrent = false)
        {
            var modUser = UserReader.GetUser(UserID);
            if (modUser == null || modUser.ID == null)
            {
                return;
            }

            modUser.UpdateVersion();
            if (ExcludeCurrent)
            {
                try
                {
                    authWorker.SetCurrentUser_ClientToken(false, modUser);
                }
                catch
                {
                    //might throw SqlException for unique key violation
                    //no need to to process error
                    //user will simply be logged out
                }

            }
        }
        /// <summary>
        /// Logout of all devices by incrementing user version
        /// </summary>
        public void LogoutOfAllDevices(string Email, bool ExcludeCurrent = false)
        {
            var modUser = UserReader.GetUser(Email);
            if (modUser == null || modUser.ID == null)
            {
                return;
            }

            modUser.UpdateVersion();
            if (ExcludeCurrent)
            {
                try
                {
                    authWorker.SetCurrentUser_ClientToken(false, modUser);
                }
                catch
                {
                    //might throw SqlException for unique key violation
                    //no need to to process error
                    //user will simply be logged out
                }
            }
        }
        /// <summary>
        /// Logout of all devices by incrementing user version
        /// </summary>
        public void LogoutOfAllDevices(string Username, string Domain, bool ExcludeCurrent = false)
        {
            var modUser = UserReader.GetUser(Username, Domain);
            if (modUser == null || modUser.ID == null)
            {
                return;
            }

            modUser.UpdateVersion();
            if (ExcludeCurrent)
            {
                try
                {
                    authWorker.SetCurrentUser_ClientToken(false, modUser);
                }
                catch
                {
                    //might throw SqlException for unique key violation
                    //no need to to process error
                    //user will simply be logged out
                }
            }
        }


        /// <summary>
        /// Remove persistent cookies from request/response
        /// </summary>
        public void TryLogOut()
        {
            TryLogOut(-1);
        }
        /// <summary>
        /// Remove persistent cookies from request/response
        /// </summary>
        internal void TryLogOut(int ErrorCode = -1, [CallerMemberName] string key = null)
        {
            bool DEBUG = false;
            if (ErrorCode != -1 && DEBUG)
            {
                CoreFactory.Singleton.Logging.CreateMessageLogAsync(ErrorCode.ToString());
            }
            if (key != null && DEBUG)
            {
                CoreFactory.Singleton.Logging.CreateMessageLogAsync(key.ToString());
            }

            try
            {

                //get cookie
                var authCookieName = CoreFactory.Singleton.Properties.AuthTknCookieName;
                HttpCookie authCookie = HttpContext.Current.Request.Cookies[authCookieName] ?? HttpContext.Current.Response.Cookies[authCookieName];


                if (authCookie != null && authCookie.Value.IsNotEmpty())
                {
                    AuthenticationToken authToken = null;
                    try
                    {
                        authToken = AuthenticationToken.Decrypt(authCookie.Value);
                        if (authToken != null)
                        {
                            TokenManager.RevokeTokenValidator(authToken);
                        }
                    }
                    catch
                    {

                    }
                }


                HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER] = null;
                if (HttpContext.Current.Request.Cookies[authCookieName] != null && HttpContext.Current.Request.Cookies[authCookieName].Value.IsNotEmpty())
                {
                    HttpContext.Current.Request.Cookies.Remove(authCookieName);
                    HttpContext.Current.Response.Cookies.Remove(authCookieName);
                    HttpContext.Current.Request.Cookies[authCookieName]?.Expire();
                }
                if (HttpContext.Current.Response.Cookies[authCookieName] != null && HttpContext.Current.Response.Cookies[authCookieName].Value.IsNotEmpty())
                {
                    HttpContext.Current.Request.Cookies.Remove(authCookieName);
                    HttpContext.Current.Response.Cookies.Remove(authCookieName);
                    HttpContext.Current.Response.Cookies[authCookieName]?.Expire();
                }


            }
            catch { }

        }




    }
}
