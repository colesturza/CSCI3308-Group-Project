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

namespace UHub.CoreLib.Security.Authentication
{
    internal abstract partial class AuthenticationProvider
    {
        /// <summary>
        /// Try to authenticate a user account using the supplied account credentials.  Includes internal logging
        /// </summary>
        /// <param name="userEmail">Email address associated with the user account</param>
        /// <param name="userPassword">Password associated with the user account</param>
        /// <param name="ResultCode">Result code to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <param name="UserTokenHandler">Success handler to handle user token distribution</param>
        /// <returns>Status Flag</returns>
        abstract internal Task<AuthResultCode> TryAuthenticateUserAsync(
            string userEmail,
            string userPassword,
            Action<Guid> GeneralFailHandler = null,
            Func<User, bool> UserTokenHandler = null);



        /// <summary>
        /// Get user auth data from cookie and load as the CurrentUser identity
        /// </summary>
        internal async Task<(User CmsUser, TokenValidationStatus TokenStatus)> ValidateAuthCookieAsync(HttpContext Context, Action ErrorHandler = null)
        {
            var authTknCookieName = CoreFactory.Singleton.Properties.AuthTknCookieName;

            //get cookie
            HttpCookie authCookie =
                    Context.Request?.Cookies.Get(authTknCookieName) ??
                    Context.Response?.Cookies.Get(authTknCookieName);

            if (authCookie == null)
            {
                ErrorHandler?.Invoke();
                var CmsUser = UserReader.GetAnonymousUser();
                var TokenStatus = TokenValidationStatus.TokenNotFound;

                return (CmsUser, TokenStatus);
            }

            if (authCookie.Value.IsEmpty())
            {
                ErrorHandler?.Invoke();
                var CmsUser = UserReader.GetAnonymousUser();
                var TokenStatus = TokenValidationStatus.TokenNotFound;

                return (CmsUser, TokenStatus);
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


            return await ValidateAuthTokenAsync(authCookie.Value, Context, ErrorHandler, succHandler);

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
        internal async Task<(User CmsUser, TokenValidationStatus TokenStatus)> ValidateAuthTokenAsync(
            string tokenStr,
            HttpContext Context,
            Action errorHandler = null,
            Action<AuthenticationToken> SuccessHandler = null)
        {

            if (tokenStr.IsEmpty())
            {
                var CmsUser = UserReader.GetAnonymousUser();
                try
                {
                    errorHandler?.Invoke();
                }
                catch { }

                return (CmsUser, TokenValidationStatus.TokenNotFound);
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

                var CmsUser = UserReader.GetAnonymousUser();
                try
                {
                    errorHandler?.Invoke();
                }
                catch { }

                return (CmsUser, TokenValidationStatus.TokenAESFailure);
            }

            return await ValidateAuthTokenAsync(authToken, Context, errorHandler, SuccessHandler);
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
        private protected async Task<(User CmsUser, TokenValidationStatus TokenStatus)> ValidateAuthTokenAsync(
            AuthenticationToken token,
            HttpContext Context,
            Action errorHandler = null,
            Action<AuthenticationToken> SuccessHandler = null)
        {

            //authenticate token
            if (token == null)
            {
                errorHandler?.Invoke();
                var CmsUser = UserReader.GetAnonymousUser();

                return (CmsUser, TokenValidationStatus.TokenNotFound);
            }

            string sessionID = GetAdjustedSessionID(token.IsPersistent, Context);


            long userID = token.UserID;
            var taskGetUser = UserReader.GetUserAsync(userID);

            var tokenStatus = await TokenManager.IsTokenValidAsync(token, sessionID);
            var isTokenValid = tokenStatus == TokenValidationStatus.Success;


            if (isTokenValid)
            {
                errorHandler?.Invoke();
                var CmsUser = UserReader.GetAnonymousUser();


                return (CmsUser, tokenStatus);
            }

            if
            (
                CoreFactory.Singleton.Properties.EnableTokenVersioning &&
                token.SystemVersion != CoreFactory.Singleton.Properties.CurrentAuthTknVersion
            )
            {
                errorHandler?.Invoke();
                var CmsUser = UserReader.GetAnonymousUser();

                return (CmsUser, TokenValidationStatus.TokenVersionMismatch);
            }



            ////////////////////////////////////////////////////////////////////////////////////////////////////
            //Validate CMS User
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            //try to get the real user from CMS DB
            User cmsUser_local = await taskGetUser;


            //validate CMS specific user
            if (cmsUser_local == null || cmsUser_local.ID == null)
            {
                errorHandler?.Invoke();
                var CmsUser = UserReader.GetAnonymousUser();

                return (CmsUser, TokenValidationStatus.TokenUserError);
            }


            //check user version
            if (CoreFactory.Singleton.Properties.EnableTokenVersioning)
            {
                string userVersion = token.UserVersion;
                if (cmsUser_local.Version != userVersion)
                {
                    errorHandler?.Invoke();
                    var CmsUser = UserReader.GetAnonymousUser();

                    return (CmsUser, TokenValidationStatus.TokenVersionMismatch);
                }
            }

            //user not confirmed
            if (!cmsUser_local.IsConfirmed)
            {
                errorHandler?.Invoke();
                var CmsUser = UserReader.GetAnonymousUser();

                return (CmsUser, TokenValidationStatus.TokenUserNotConfirmed);
            }
            if (!cmsUser_local.IsApproved)
            {
                errorHandler?.Invoke();
                var CmsUser = UserReader.GetAnonymousUser();


                return (CmsUser, TokenValidationStatus.TokenUserNotApproved);
            }

            //user disabled for some other reason
            //could be disabled by admin
            if (!cmsUser_local.IsEnabled)
            {
                errorHandler?.Invoke();
                var CmsUser = UserReader.GetAnonymousUser();

                return (CmsUser, TokenValidationStatus.TokenUserDisabled);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            //Validation passed
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            SuccessHandler?.Invoke(token);

            return (cmsUser_local, TokenValidationStatus.Success);

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
        internal async Task<AuthenticationToken> GenerateAuthTokenAsync(bool IsPersistent, User cmsUser, HttpContext Context)
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
            string sessionID = GetAdjustedSessionID(isPersistent, Context);


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
                await TokenManager.SaveTokenValidatorToDBAsync(authToken);
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

        internal async Task<bool> IsUserLockedOutAsync(long UserID)
        {
            var info = await GetUserAuthInfo_DBAsync(UserID);
            if (info == null)
            {
                throw new InvalidOperationException("User does not exist");
            }


            var maxPsdAttmpt = CoreFactory.Singleton.Properties.MaxPswdAttempts;

            return info.FailedPswdAttemptCount >= maxPsdAttmpt;
        }
        #endregion IsUserLocked


        #region GetUserLockoutDate
        internal async Task<DateTimeOffset> GetUserLockoutDateAsync(long UserID)
        {
            var info = await GetUserAuthInfo_DBAsync(UserID);
            if (info == null)
            {
                throw new InvalidOperationException("User does not exist");
            }


            return info.LastLockoutDate ?? DateTimeOffset.MinValue;
        }
        #endregion GetUserLockoutDate


        #region LockoutUser
        private protected async Task LockoutUserAsync(long UserID)
        {

            await SqlWorker.ExecNonQueryAsync(
               CoreFactory.Singleton.Properties.CmsDBConfig,
               "[dbo].[User_SetLastLockoutDateByID]",
               (cmd) =>
               {
                   cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
               });
        }
        #endregion LockoutUser


        #region ResetLockout
        internal async Task ResetUserLockoutAsync(long UserID)
        {

            await SqlWorker.ExecNonQueryAsync(
               CoreFactory.Singleton.Properties.CmsDBConfig,
               "[dbo].[User_ResetFailedPswdAttemptsByID]",
               (cmd) =>
               {
                   cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
               });
        }
        #endregion ResetLockout


        #region HandleBadPswd

        private protected async Task HandleBadPswdAttemptAsync(UserAuthInfo UserAuthInfo)
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


            //initialize error state
            List<Task> taskSet = new List<Task>();
            if ((dt + psdAttmptPrd) < FailoverDateTimeOffset.UtcNow)
            {
                failCount = 0;
                taskSet.Add(ResetUserLockoutAsync(UserAuthInfo.UserID));
            }
            //advance error state
            failCount++;
            taskSet.Add(LogFailedPsdAttemptAsync(UserAuthInfo.UserID));


            if (failCount >= maxPsdAttmpt)
            {
                taskSet.Add(LockoutUserAsync(UserAuthInfo.UserID));
            }


            await Task.WhenAll(taskSet);
        }
        #endregion HandleBadPsd


        #region LogFailedPswd
        private protected async Task LogFailedPsdAttemptAsync(long UserID)
        {

            await SqlWorker.ExecNonQueryAsync(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_LogFailedPswdAttemptByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                });
        }
        #endregion LogFailedPswd


        #region UserAuthInfo
        private protected async Task<UserAuthInfo> GetUserAuthInfo_DBAsync(long UserID)
        {

            var temp = SqlWorker.ExecBasicQueryAsync<UserAuthInfo>(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_GetAuthInfoByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                });


            return (await temp).SingleOrDefault();
        }

        private protected async Task<UserAuthInfo> GetUserAuthInfo_DBAsync(string Email)
        {

            var temp = SqlWorker.ExecBasicQueryAsync<UserAuthInfo>(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_GetAuthInfoByEmail]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                });


            return (await temp).SingleOrDefault();
        }


        private protected async Task<UserAuthInfo> GetUserAuthInfo_DBAsync(string Username, string Domain)
        {

            var temp = SqlWorker.ExecBasicQueryAsync<UserAuthInfo>(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_GetAuthInfoByUsername]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = Username;
                    cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = Domain;
                });


            return (await temp).SingleOrDefault();
        }

        #endregion UserAuthInfo
    }
}
