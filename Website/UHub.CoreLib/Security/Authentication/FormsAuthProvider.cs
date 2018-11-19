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
using UHub.CoreLib.Entities.Users.Management;
using System.Text.RegularExpressions;

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
        override internal AuthResultCode TryAuthenticateUser(
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


            var attrIsValid = Shared.TryAuthenticate_ValidateFields(UserEmail, UserPassword);
            if (attrIsValid != AuthResultCode.Success)
            {
                return attrIsValid;
            }


            //get userAuth info (pswf info)
            UserAuthInfo userAuthInfo = null;
            try
            {
                userAuthInfo = GetUserAuthInfo_DB(UserEmail);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex);
                GeneralFailHandler?.Invoke(new Guid("B61DB416-F38E-495C-BFDF-317FDB7F8063"));
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
                    return AuthResultCode.UnknownError;
                    GeneralFailHandler?.Invoke(new Guid("FC5D3DDB-A48B-49C9-922E-7A96CB53CA7E"));
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

                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex);
                GeneralFailHandler?.Invoke(new Guid("EC7D4ACA-498F-49DA-994B-099292AA9BD8"));
                return AuthResultCode.UnknownError;
            }


            var userAccessIsValid = Shared.TryAuthenticate_ValidateUserAccess(cmsUser);
            if (userAccessIsValid != AuthResultCode.Success)
            {
                return userAccessIsValid;
            }



            var status = UserTokenHandler?.Invoke(cmsUser) ?? true;
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



        private PasswordValidationStatus ValidatePassword(string UserEmail, string Password)
        {
            var userAuthInfo = GetUserAuthInfo_DB(UserEmail);


            if (userAuthInfo == null)
            {
                return PasswordValidationStatus.NotFound;
            }
            if (userAuthInfo.PswdHash.IsEmpty())
            {
                return PasswordValidationStatus.NotFound;
            }
            if (userAuthInfo.Salt.IsEmpty())
            {
                return PasswordValidationStatus.NotFound;
            }


            //check pswd expiration
            var maxPsdAge = CoreFactory.Singleton.Properties.MaxPswdAge;

            if (maxPsdAge != null && maxPsdAge.Ticks != 0)
            {
                var pswdModDate = userAuthInfo.PswdModifiedDate;
                var maxValidDt = pswdModDate.Add(maxPsdAge);
                var now = FailoverDateTimeOffset.UtcNow;
                if (maxValidDt < now)
                {
                    return PasswordValidationStatus.PswdExpired;
                }
            }


            //check pswd hashes
            bool isMatch = false;
            var hashType = CoreFactory.Singleton.Properties.PswdHashType;

            if (hashType == CryptoHashType.Bcrypt)
            {
                isMatch = BCrypt.Net.BCrypt.Verify(Password, userAuthInfo.PswdHash);
            }
            else
            {
                isMatch = (userAuthInfo.PswdHash == Password.GetCryptoHash(hashType, userAuthInfo.Salt));
            }


            //process result
            if (!isMatch)
            {
                HandleBadPswdAttempt(userAuthInfo);
                return PasswordValidationStatus.HashMismatch;
            }
            else
            {
                return PasswordValidationStatus.Success;
            }


        }


    }
}
