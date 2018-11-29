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

namespace UHub.CoreLib.Security.Authentication.Providers.Forms
{
    internal sealed partial class FormsAuthProvider
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
        override internal AuthResultCode TryAuthenticateUser(
            string UserEmail,
            string UserPassword,
            Func<User, bool> UserTokenHandler = null)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            UserEmail = UserEmail?.Trim();


            var attrIsValid = Shared.TryAuthenticate_ValidateFields(UserEmail, UserPassword);
            if (attrIsValid != 0)
            {
                return attrIsValid;
            }


            //get userAuth info (pswf info)
            UserAuthData userAuthInfo = null;
            try
            {
                userAuthInfo = UserReader.TryGetUserAuthData(UserEmail);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("78B4CACB-D93D-4D3B-945D-923676D05A91", ex);
                return AuthResultCode.UnknownError;
            }

            //ensure account exists
            if (userAuthInfo == null)
            {
                return AuthResultCode.UserInvalid;
            }


            if (userAuthInfo.IsLockedOut)
            {
                var lastLock = userAuthInfo.LastLockoutDate.Value;
                var resetDt = lastLock.Add(CoreFactory.Singleton.Properties.PswdLockResetPeriod);


                var now = FailoverDateTimeOffset.UtcNow;
                if (resetDt < now)
                {
                    try
                    {
                        ResetUserLockout(userAuthInfo.UserID);
                    }
                    catch (Exception ex)
                    {
                        CoreFactory.Singleton.Logging.CreateErrorLogAsync("6E63EB15-1A36-4D12-95F7-693B7F9A9AE3", ex);
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
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("CC051A0E-3530-4DAB-8654-2FEA5DB40332", ex);
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
                    CoreFactory.Singleton.Logging.CreateErrorLogAsync("E4D786E2-6574-4C11-95BF-06FF2BA069B1");
                    return AuthResultCode.UnknownError;
                }
            }




            //try to get the real user from CMS DB
            User cmsUser;
            try
            {
                cmsUser = UserReader.GetUser(userAuthInfo.UserID);
            }
            catch (Exception ex)
            {

                CoreFactory.Singleton.Logging.CreateErrorLogAsync("D1CBD896-CAB8-48AD-9DA6-DD2DE9CEA2A1", ex);
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
