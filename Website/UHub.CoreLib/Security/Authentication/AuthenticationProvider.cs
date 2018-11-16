using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Logging;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Entities.Users.Management;
using System.Runtime.CompilerServices;

namespace UHub.CoreLib.Security.Authentication
{
    internal abstract partial class AuthenticationProvider
    {
        internal AuthenticationProvider()
        {

        }

        /// <summary>
        /// Try to authenticate a user account using the supplied account credentials.  Includes internal logging
        /// </summary>
        /// <param name="userEmail">Email address associated with the user account</param>
        /// <param name="userPassword">Password associated with the user account</param>
        /// <param name="ResultCode">Result code to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <param name="UserTokenHandler">Success handler to handle user token distribution</param>
        /// <returns>Status Flag</returns>
        abstract internal bool TryAuthenticateUser(
            string userEmail, 
            string userPassword,
            out AuthResultCode ResultCode,
            Action<Guid> GeneralFailHandler = null,
            Func<User, bool> UserTokenHandler = null);

        /// <summary>
        /// Set current request user for caching
        /// </summary>
        /// <param name="CmsUser"></param>
        internal void SetCurrentUser_Local(User CmsUser)
        {
            //set identity
            HttpContext.Current.Items[AuthenticationManager.REQUEST_CURRENT_USER] = CmsUser;
        }


        /// <summary>
        /// Set current user via cookie - allows login to persist between requests
        /// </summary>
        /// <param name="IsPersistentent">Is token persisten</param>
        /// <param name="CmsUser">User to set</param>
        /// <param name="SystemVersion">Specify an Auth Tkn version number. Defaults to <see cref="CmsProperties.CurrentAuthTknVersion"/> </param>
        /// <exception cref="SqlException"></exception>
        /// /// <exception cref="Exception"></exception>
        internal void SetCurrentUser_ClientToken(bool IsPersistentent, User CmsUser)
        {
            var context = HttpContext.Current;
            var token = GenerateAuthToken(IsPersistentent, CmsUser, context);
            SetCurrentUser_ClientToken(token);
        }

        /// <summary>
        /// Set current user via cookie - allows login to persist between requests
        /// </summary>
        /// <param name="token"></param>
        private protected void SetCurrentUser_ClientToken(AuthenticationToken token)
        {
            var authTknCookieName = CoreFactory.Singleton.Properties.AuthTknCookieName;


            //remove old token
            HttpContext.Current.Request.Cookies.Remove(authTknCookieName);
            HttpContext.Current.Response.Cookies.Remove(authTknCookieName);


            string encryptedToken = token.Encrypt();
            HttpCookie authCookie = new HttpCookie(authTknCookieName, encryptedToken);
            authCookie.Shareable = false;
            //set expiration for persistent cookies
            //otherwise the cookie will expire with browser session
            if (token.IsPersistent)
            {
                authCookie.Expires = token.ExpirationDate.UtcDateTime;
            }

            HttpContext.Current.Response.Cookies.Add(authCookie);
        }


        /// <summary>
        /// Get user auth data from cookie and load as the CurrentUser identity
        /// </summary>
        internal void ValidateAuthCookie(out User CmsUser, out TokenValidationStatus TokenStatus, Action ErrorHandler = null)
        {
            var authTknCookieName = CoreFactory.Singleton.Properties.AuthTknCookieName;

            //get cookie
            HttpCookie authCookie =
                    HttpContext.Current?.Request?.Cookies.Get(authTknCookieName) ??
                    HttpContext.Current?.Response?.Cookies.Get(authTknCookieName);

            if (authCookie == null)
            {
                ErrorHandler?.Invoke();
                CmsUser = UserReader.GetAnonymousUser();
                TokenStatus = TokenValidationStatus.TokenNotFound;
                return;
            }

            if (authCookie.Value.IsEmpty())
            {
                ErrorHandler?.Invoke();
                CmsUser = UserReader.GetAnonymousUser();
                TokenStatus = TokenValidationStatus.TokenNotFound;
                return;
            }


            var enableAuthSlide = CoreFactory.Singleton.Properties.EnableAuthTokenSlidingExpiration;
            Action<AuthenticationToken> succHandler =
                (token) =>
                {
                    //RE-DATE AUTH TICKET
                    //custom sliding expiration
                    if (enableAuthSlide)
                    {
                        var isSlide = SlideAuthTokenExpiration(token);

                        if (isSlide)
                        {
                            SetCurrentUser_ClientToken(token);
                        }
                    }
                };


            ValidateAuthToken(authCookie.Value, out CmsUser, out TokenStatus, ErrorHandler, succHandler);

        }

        /// <summary>
        /// Check if authentication token is valid
        /// </summary>
        /// <param name="tokenStr"></param>
        /// <param name="CmsUser"></param>
        /// <param name="tokenStatus"></param>
        /// <param name="errorHandler"></param>
        /// <param name="SuccessHandler"></param>
        /// <returns></returns>
        internal bool ValidateAuthToken(
            string tokenStr,
            out User CmsUser,
            out TokenValidationStatus tokenStatus,
            Action errorHandler = null,
            Action<AuthenticationToken> SuccessHandler = null)
        {

            if (tokenStr.IsEmpty())
            {
                CmsUser = UserReader.GetAnonymousUser();
                tokenStatus = TokenValidationStatus.TokenNotFound;
                try
                {
                    errorHandler?.Invoke();
                }
                catch { }
                return false;
            }
            //get token
            AuthenticationToken authToken = null;
            try
            {
                authToken = AuthenticationToken.Decrypt(tokenStr);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex);
                CmsUser = UserReader.GetAnonymousUser();
                tokenStatus = TokenValidationStatus.TokenAESFailure;
                try
                {
                    errorHandler?.Invoke();
                }
                catch { }
                return false;
            }

            return ValidateAuthToken(authToken, out CmsUser, out tokenStatus, errorHandler, SuccessHandler);
        }

        /// <summary>
        /// Check if authentication token is valid
        /// </summary>
        /// <param name="token"></param>
        /// <param name="CmsUser"></param>
        /// <param name="tokenStatus"></param>
        /// <param name="errorHandler"></param>
        /// <param name="SuccessHandler"></param>
        /// <returns></returns>
        private protected bool ValidateAuthToken(
            AuthenticationToken token,
            out User CmsUser,
            out TokenValidationStatus tokenStatus,
            Action errorHandler = null,
            Action<AuthenticationToken> SuccessHandler = null)
        {

            //authenticate token
            if (token == null)
            {
                errorHandler?.Invoke();
                CmsUser = UserReader.GetAnonymousUser();
                tokenStatus = TokenValidationStatus.TokenNotFound;
                return false;
            }

            var context = HttpContext.Current;

            string sessionID = GetAdjustedSessionID(token.IsPersistent, context);

            var tempTokenStatus = TokenManager.IsTokenValid(token, sessionID);
            if (tempTokenStatus != TokenValidationStatus.Success)
            {
                errorHandler?.Invoke();
                CmsUser = UserReader.GetAnonymousUser();
                tokenStatus = tempTokenStatus;
                return false;
            }

            if
            (
                CoreFactory.Singleton.Properties.EnableTokenVersioning &&
                token.SystemVersion != CoreFactory.Singleton.Properties.CurrentAuthTknVersion
            )
            {
                errorHandler?.Invoke();
                CmsUser = UserReader.GetAnonymousUser();
                tokenStatus = TokenValidationStatus.TokenVersionMismatch;
                return false;
            }


            long userID = token.UserID;
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            //Validate CMS User
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            //try to get the real user from CMS DB
            User cmsUser_local;
            try
            {
                cmsUser_local = UserReader.GetUser(userID);
            }
            catch
            {

                errorHandler?.Invoke();
                CmsUser = UserReader.GetAnonymousUser();
                tokenStatus = TokenValidationStatus.TokenUserError;
                return false;
            }

            //validate CMS specific user
            if (cmsUser_local == null || cmsUser_local.ID == null)
            {
                errorHandler?.Invoke();
                CmsUser = UserReader.GetAnonymousUser();
                tokenStatus = TokenValidationStatus.TokenUserError;
                return false;
            }

            //check user version
            if (CoreFactory.Singleton.Properties.EnableTokenVersioning)
            {
                string userVersion = token.UserVersion;
                if (cmsUser_local.Version != userVersion)
                {
                    errorHandler?.Invoke();
                    CmsUser = UserReader.GetAnonymousUser();
                    tokenStatus = TokenValidationStatus.TokenVersionMismatch;
                    return false;
                }
            }

            //user not confirmed
            if (!cmsUser_local.IsConfirmed)
            {
                errorHandler?.Invoke();
                CmsUser = UserReader.GetAnonymousUser();
                tokenStatus = TokenValidationStatus.TokenUserNotConfirmed;
                return false;
            }
            if (!cmsUser_local.IsApproved)
            {
                errorHandler?.Invoke();
                CmsUser = UserReader.GetAnonymousUser();
                tokenStatus = TokenValidationStatus.TokenUserNotApproved;
                return false;
            }

            //user disabled for some other reason
            //could be disabled by admin
            if (!cmsUser_local.IsEnabled)
            {
                errorHandler?.Invoke();
                CmsUser = UserReader.GetAnonymousUser();
                tokenStatus = TokenValidationStatus.TokenUserDisabled;
                return false;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            //Validation passed
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            SuccessHandler?.Invoke(token);
            CmsUser = cmsUser_local;
            tokenStatus = TokenValidationStatus.Success;
            return true;

        }

        /// <summary>
        /// Get real session ID from web client or use the get the ID presented by the MACHINE_KEY attribute
        /// </summary>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        private protected string GetAdjustedSessionID(bool isPersistent, HttpContext Context)
        {
            string sessionID = "";
            string clientKey = "";
            clientKey = Context?.Request?.Headers[Common.AUTH_HEADER_MACHINE_KEY] ?? "";

            //session ID must be ignored for persistent connections
            //otherwise token will not work if user comes back to site at later date
            if (isPersistent)
            {
                if (clientKey.IsNotEmpty())
                {
                    return clientKey;
                }
                else
                {
                    return "";
                }
            }

            //NOT PERSISTENT
            if (clientKey.IsNotEmpty())
            {
                sessionID = clientKey;
            }
            else
            {
                var sessionCookie = CoreFactory.Singleton.Properties.SessionIDCookieName;

                //get session directly from cookie because Context.Session is not initialized yet
                sessionID = 
                    Context?.Request?.Cookies?.Get(sessionCookie)?.Value
                    ?? "";
            }

            //sterilize for token processing
            sessionID = sessionID.Replace('|', '0');
            return sessionID;
        }

        /// <summary>
        /// Slide the expiration date for an auth token
        /// </summary>
        /// <param name="token"></param>
        internal bool SlideAuthTokenExpiration(AuthenticationToken token)
        {
            if (token.IsPersistent)
            {
                //persistent tokens are already set to max possible age
                //no slide is necessary
                return false;
            }

            //ensure that the new expiration date is inside the max lifespan window
            var timeout = CoreFactory.Singleton.Properties.AuthTokenTimeout;
            var slideDate = FailoverDateTimeOffset.UtcNow.Add(timeout);
            DateTimeOffset maxExpire;


            var maxLifespan = CoreFactory.Singleton.Properties.MaxAuthTokenLifespan;
            if (maxLifespan.Ticks == 0)
            {
                maxExpire = DateTimeOffset.MaxValue;
            }
            else
            {
                maxExpire = token.IssueDate.Add(maxLifespan);
            }


            if (slideDate <= maxExpire)
            {
                token.SetExpiration(slideDate);
                return true;
            }
            if (slideDate > maxExpire)
            {
                if (token.ExpirationDate == maxExpire)
                {
                    return false;
                }
                token.SetExpiration(maxExpire);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Generate new auth token for user and write validator to DB
        /// </summary>
        /// <param name="IsPersistent"></param>
        /// <param name="cmsUser"></param>
        /// <param name="SystemVersion"></param>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        internal AuthenticationToken GenerateAuthToken(bool IsPersistent, User cmsUser, HttpContext context)
        {
            if (cmsUser.ID == null)
            {
                throw new InvalidOperationException("User not valid");
            }

            //Only set persistent tokens if it is allowed by the CMS config
            var isPersistent = CoreFactory.Singleton.Properties.EnablePersistentAuthTokens && IsPersistent;


            var issue = FailoverDateTimeOffset.UtcNow;
            var ID = cmsUser.ID.Value;
            var sysVersion = CoreFactory.Singleton.Properties.CurrentAuthTknVersion;
            string userVersion = cmsUser.Version;
            string sessionID = GetAdjustedSessionID(isPersistent, context);


            var maxTknLifespan = CoreFactory.Singleton.Properties.MaxAuthTokenLifespan;
            var authTknTimeout = CoreFactory.Singleton.Properties.AuthTokenTimeout;

            DateTimeOffset expiration;
            if (maxTknLifespan.Ticks == 0)
            {
                expiration = DateTimeOffset.MaxValue;
            }
            else
            {
                var tSpan = isPersistent ? maxTknLifespan : authTknTimeout;
                expiration = FailoverDateTimeOffset.UtcNow.Add(tSpan);
            }


            AuthenticationToken authToken = new AuthenticationToken(isPersistent, issue, expiration, ID, sysVersion, userVersion, sessionID);
            

            try
            {
                TokenManager.SaveTokenValidatorToDB(authToken);
            }
            catch
            {
                //DB error
                //insert could potentially violate unique key constraints
                throw;
            }

            return authToken;
        }


        #region IsUserLocked

        internal bool IsUserLockedOut(long UserID)
        {
            var info = GetUserAuthInfo_DB(UserID);
            if (info == null)
            {
                throw new InvalidOperationException("User does not exist");
            }


            var maxPsdAttmpt = CoreFactory.Singleton.Properties.MaxPswdAttempts;

            return info.FailedPswdAttemptCount >= maxPsdAttmpt;
        }
        #endregion IsUserLocked


        #region GetUserLockoutDate
        internal DateTimeOffset GetUserLockoutDate(long UserID)
        {
            var info = GetUserAuthInfo_DB(UserID);
            if (info == null)
            {
                throw new InvalidOperationException("User does not exist");
            }


            return info.LastLockoutDate ?? DateTimeOffset.MinValue;
        }
        #endregion GetUserLockoutDate


        #region LockoutUser
        private protected void LockoutUser(long UserID)
        {

            SqlWorker.ExecNonQuery(
               CoreFactory.Singleton.Properties.CmsDBConfig,
               "[dbo].[User_SetLastLockoutDateByID]",
               (cmd) =>
               {
                   cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
               });
        }
        #endregion LockoutUser


        #region ResetLockout
        internal void ResetUserLockout(long UserID)
        {

            SqlWorker.ExecNonQuery(
               CoreFactory.Singleton.Properties.CmsDBConfig,
               "[dbo].[User_ResetFailedPswdAttemptsByID]",
               (cmd) =>
               {
                   cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
               });
        }
        #endregion ResetLockout


        #region HandleBadPswd

        private protected void HandleBadPswdAttempt(UserAuthInfo UserAuthInfo)
        {

            //log failed pswd attempt to DB
            //CoreFactory.Singleton.Logging.CreateDBActivityLog(ActivityLogType.UserLoginFailure);

            var failCount = UserAuthInfo.FailedPswdAttemptCount;
            var maxPsdAttmpt = CoreFactory.Singleton.Properties.MaxPswdAttempts;


            //do nothing if the account is already locked
            if (failCount >= maxPsdAttmpt)
            {
                return;
            }

            //check if the current failure is outside the failure attempt window
            //if not, then reset lockout window (but only if the account is not already locked)

            var psdAttmptPrd = CoreFactory.Singleton.Properties.PswdAttemptPeriod;

            var dt = UserAuthInfo.StartFailedPswdWindow ?? DateTimeOffset.MinValue;
            if ((dt + psdAttmptPrd) < FailoverDateTimeOffset.UtcNow)
            {
                failCount = 0;
                ResetUserLockout(UserAuthInfo.UserID);
                failCount++;
                LogFailedPsdAttempt(UserAuthInfo.UserID);
            }
            else
            {
                failCount++;
                LogFailedPsdAttempt(UserAuthInfo.UserID);
            }


            if (failCount >= maxPsdAttmpt)
            {
                LockoutUser(UserAuthInfo.UserID);
            }
        }
        #endregion HandleBadPsd


        #region LogFailedPswd
        private protected void LogFailedPsdAttempt(long UserID)
        {

            SqlWorker.ExecNonQuery(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_LogFailedPswdAttemptByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                });
        }
        #endregion LogFailedPswd


        #region UserAuthInfo
        private protected UserAuthInfo GetUserAuthInfo_DB(long UserID)
        {

            return SqlWorker.ExecBasicQuery<UserAuthInfo>(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_GetAuthInfoByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                })
                .SingleOrDefault();
        }

        private protected UserAuthInfo GetUserAuthInfo_DB(string Email)
        {

            return SqlWorker.ExecBasicQuery<UserAuthInfo>(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_GetAuthInfoByEmail]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                })
                .SingleOrDefault();
        }


        private protected UserAuthInfo GetUserAuthInfo_DB(string Username, string Domain)
        {

            return SqlWorker.ExecBasicQuery<UserAuthInfo>(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_GetAuthInfoByUsername]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = Username;
                    cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = Domain;
                })
                .SingleOrDefault();
        }

        #endregion UserAuthInfo
    }
}
