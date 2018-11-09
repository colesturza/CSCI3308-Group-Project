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
        /// Validate user credentials then set authentication token via cookie
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <param name="IsPersistent">Flag to set token persistence status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        public async Task<AuthResultCode> TrySetClientAuthTokenAsync(
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


            return await authWorker.TryAuthenticateUserAsync(
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
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        public async Task<(string AuthToken, AuthResultCode ResultCode)> TryGetClientAuthTokenAsync(
            string UserEmail,
            string UserPassword,
            bool IsPersistent,
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
                    var tkn = authWorker.GenerateAuthToken(IsPersistent, cmsUser);
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


            var resultCode = await authWorker.TryAuthenticateUserAsync(
                UserEmail,
                UserPassword,
                GeneralFailHandler,
                userTokenHandler);


            return (token, resultCode);
        }


        /// <summary>
        /// Slide the expiration date of a token and return a new encrypted client token
        /// <para/> If token cannot be extend, then the original token is returned
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<(string Token, TokenValidationStatus TokenStatus)> TrySlideAuthTokenExpirationAsync(string token)
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

            var result = await authWorker.ValidateAuthTokenAsync(token, () => { TryLogOut(5); }, succHandler);


            return (token, result.TokenStatus);
        }

        /// <summary>
        /// Try to authenticate a user account using the supplied account credentials
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        public async Task<AuthResultCode> TryAuthenticateUserAsync(
            string UserEmail,
            string UserPassword,
            Action<Guid> GeneralFailHandler = null)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            return await authWorker.TryAuthenticateUserAsync(
                UserEmail,
                UserPassword,
                GeneralFailHandler,
                null);
        }



        /// <summary>
        /// Set the user for the current request using an Auth Token.  If the token is invalid, then the user will be set to ANON privs.
        /// This method is provided so that restful API requests without an auth cookie can be authenticated throughout the CMS pipeline
        /// </summary>
        /// <param name="tokenStr">AuthToken in string form</param>
        /// <returns>Status flag</returns>
        public async Task<(bool StatusFlag, TokenValidationStatus TokenStatus)> TrySetRequestUserAsync(string tokenStr)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            var validationResult = await ValidateAuthTokenAsync(tokenStr);
            var cmsUser = validationResult.CmsUser;
            var tokenStatus = validationResult.TokenStatus;
            var isValid = (tokenStatus == TokenValidationStatus.Success);


            if (!isValid)
            {
                cmsUser = UserReader.GetAnonymousUser();
                HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER] = cmsUser;
                return (false, tokenStatus);
            }


            if (cmsUser == null || cmsUser.ID == null)
            {
                cmsUser = UserReader.GetAnonymousUser();
                HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER] = cmsUser;
                return (false, tokenStatus);
            }


            HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER] = cmsUser;


            var isSuccess = cmsUser.ID != null;
            return (isSuccess, tokenStatus);
        }

        /// <summary>
        /// Ensure that auth token is valid and user is logged in
        /// </summary>
        /// <param name="tokenStr">Auth token in string form</param>
        /// <param name="CmsUser">User encapsulated by auth token (if valid)</param>
        /// <param name="tokenStatus">Return auth token validation status</param>
        /// <returns></returns>
        public async Task<(User CmsUser, TokenValidationStatus TokenStatus)> ValidateAuthTokenAsync(string tokenStr)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return await authWorker.ValidateAuthTokenAsync(tokenStr, null);
        }


        /// <summary>
        /// Determine if there is a user logged in
        /// Uses the auth cookie and various expiration timers
        /// Returns the authenticated user or a reference to Anon instance
        /// </summary>
        /// <returns></returns>
        public async Task<(bool StatusFlag, User CmsUser, TokenValidationStatus TokenStatus)> IsUserLoggedInAsync()
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            var requestUser = HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER];

            if (false && requestUser != null && requestUser is User currentUser)
            {
                var CmsUser = currentUser;
                if (currentUser.ID == null)
                {
                    var tokenStatus = TokenValidationStatus.AnonUser;
                    return (false, CmsUser, tokenStatus);
                }

                return (true, CmsUser, TokenValidationStatus.Success);
            }
            else
            {
                var authData = await authWorker.ValidateAuthCookieAsync();
                var CmsUser = authData.CmsUser;
                var TokenStatus = authData.TokenStatus;


                //check for real user vs Anon user
                if (CmsUser == null || CmsUser.ID == null)
                {
                    if (TokenStatus == TokenValidationStatus.Success)
                    {
                        TokenStatus = TokenValidationStatus.AnonUser;
                    }

                    CmsUser = UserReader.GetAnonymousUser();
                    HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER] = CmsUser;

                    return (false, CmsUser, TokenStatus);
                }


                //GOOD USER
                HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER] = CmsUser;



                TokenStatus = TokenValidationStatus.Success;
                return (true, CmsUser, TokenStatus);
            }

        }


        /// <summary>
        /// Get the currently authenticated CMS user. If the user is not authenticated, then an anonymous user is returned (UID=null, class=Anon)
        /// </summary>
        /// <returns></returns>
        public async Task<(User CmsUser, TokenValidationStatus TokenStatus)> GetCurrentUserAsync()
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
                    return (currentUser, TokenValidationStatus.Success);
                }
                else
                {

                    var authData = await authWorker.ValidateAuthCookieAsync();
                    var CmsUser = authData.CmsUser;
                    var TokenStatus = authData.TokenStatus;


                    //BAD USER
                    if (CmsUser == null || CmsUser.ID == null)
                    {
                        if (TokenStatus == TokenValidationStatus.Success)
                        {
                            TokenStatus = TokenValidationStatus.AnonUser;
                        }
                        CmsUser = UserReader.GetAnonymousUser();
                        HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER] = CmsUser;


                    }
                    else
                    {
                        //GOOD USER
                        HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER] = CmsUser;

                    }


                    return (CmsUser, TokenStatus);
                }

            }
            catch (Exception ex)
            {
                var CmsUser = UserReader.GetAnonymousUser();
                return (CmsUser, TokenValidationStatus.AnonUser);
            }
        }




    }
}
