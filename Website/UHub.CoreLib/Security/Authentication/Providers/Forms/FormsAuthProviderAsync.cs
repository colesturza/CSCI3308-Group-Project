using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Logging;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Entities.Users.DataInterop;
using System.Text.RegularExpressions;
using System.Web;

namespace UHub.CoreLib.Security.Authentication.Providers.Forms
{
    internal sealed partial class FormsAuthProvider : AuthenticationProvider
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
        override internal async Task<AuthResultCode> TryAuthenticateUserAsync(
            string UserEmail,
            string UserPassword,
            Func<User, bool> UserTokenHandler = null)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            UserEmail = UserEmail?.Trim();
            var taskGetUserAuthInfo = UserReader.TryGetUserAuthDataAsync(UserEmail);


            var attrIsValid = Shared.TryAuthenticate_ValidateFields(UserEmail, UserPassword);
            if (attrIsValid != 0)
            {
                return attrIsValid;
            }



            //get userAuth info (pswf info)
            UserAuthData userAuthInfo = null;
            try
            {
                userAuthInfo = await taskGetUserAuthInfo;
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("2645E23E-6171-487A-AE7C-8970511C3390", ex);
                return AuthResultCode.UnknownError;
            }
            //ensure account exists
            if (userAuthInfo == null)
            {
                return AuthResultCode.UserInvalid;
            }



            //BEGIN ASYNC GET USER
            var taskGetUser = UserReader.GetUserAsync(userAuthInfo.UserID);



            if (userAuthInfo.IsLockedOut)
            {
                var lastLock = userAuthInfo.LastLockoutDate.Value;
                var resetDt = lastLock.Add(CoreFactory.Singleton.Properties.PswdLockResetPeriod);


                var now = FailoverDateTimeOffset.UtcNow;
                if (resetDt < now)
                {
                    try
                    {
                        await ResetUserLockoutAsync(userAuthInfo.UserID);
                    }
                    catch (Exception ex)
                    {
                        CoreFactory.Singleton.Logging.CreateErrorLogAsync("A53922C5-0923-4382-A572-50ED87A0B2BE", ex);
                        return AuthResultCode.UnknownError;
                    }
                }
                else
                {
                    return AuthResultCode.UserLocked;
                }
            }

            //check name/password combo
            PasswordValidationStatus validationStatus = PasswordValidationStatus.UnknownError;
            try
            {
                validationStatus = ValidatePassword(UserEmail, UserPassword);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("55AE48DF-B91A-4B24-8EED-2077493BEFE8", ex);
                return AuthResultCode.UnknownError;
            }
            if (validationStatus != PasswordValidationStatus.Success)
            {

                if (validationStatus == PasswordValidationStatus.PswdExpired)
                {
                    return AuthResultCode.PswdExpired;
                }
                else if (validationStatus == PasswordValidationStatus.InvalidUser)
                {
                    return AuthResultCode.UserInvalid;
                }
                else if (validationStatus == PasswordValidationStatus.HashMismatch)
                {
                    return AuthResultCode.CredentialsInvalid;
                }
                else
                {
                    CoreFactory.Singleton.Logging.CreateErrorLogAsync("07F533B2-FBAD-4751-A467-494BA2D6D19D");
                    return AuthResultCode.UnknownError;
                }

            }




            //try to get the real user from CMS DB
            User cmsUser = null;
            try
            {
                cmsUser = await taskGetUser;
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("D7101F53-4E23-4786-A1C2-69CC6E5EFA25", ex);
                return AuthResultCode.UnknownError;
            }

            var userAccessIsValid = Shared.TryAuthenticate_ValidateUserAccess(cmsUser);
            if (userAccessIsValid != AuthResultCode.Success)
            {
                return userAccessIsValid;
            }


            var status = true;
            if (UserTokenHandler != null)
            {
                status = UserTokenHandler.Invoke(cmsUser);
            }


            if (status)
            {
                //CoreFactory.Singleton.Logging.CreateDBActivityLog(ActivityLogType.UserLogin);
                return AuthResultCode.Success;
            }
            else
            {
                return AuthResultCode.UnknownError;
            }

        }


    }
}
