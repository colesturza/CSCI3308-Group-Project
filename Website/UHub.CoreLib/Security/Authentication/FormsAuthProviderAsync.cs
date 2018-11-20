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

namespace UHub.CoreLib.Security.Authentication
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
            Action<Guid> GeneralFailHandler = null,
            Func<User, bool> UserTokenHandler = null)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            UserEmail = UserEmail?.Trim();
            var taskGetUserAuthInfo = GetUserAuthInfo_DBAsync(UserEmail);


            var attrIsValid = Shared.TryAuthenticate_ValidateFields(UserEmail, UserPassword);
            if (attrIsValid != AuthResultCode.Success)
            {
                return attrIsValid;
            }



            //get userAuth info (pswf info)
            UserAuthInfo userAuthInfo = await taskGetUserAuthInfo;
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
                    ResetUserLockout(userAuthInfo.UserID);
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
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex);
                GeneralFailHandler?.Invoke(new Guid("EE2C6BB0-8E49-4B31-9CB1-8C246C3EFFD9"));

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
                    GeneralFailHandler?.Invoke(new Guid("FC5D3DDB-A48B-49C9-922E-7A96CB53CA7E"));
                    return AuthResultCode.UnknownError;
                }

            }




            //try to get the real user from CMS DB
            User cmsUser = await taskGetUser;

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
