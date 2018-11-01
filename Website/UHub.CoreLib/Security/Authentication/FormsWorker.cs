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
    internal sealed class FormsWorker : AuthenticationWorker
    {
        /// <summary>
        /// Try to authenticate a user account using the supplied account credentials.  Includes internal logging
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <param name="EmailEmptyHandler">Error handler in case the user email is null or empty</param>
        /// <param name="EmailInvalidHandler">Error handler in case email is not in valid format</param>
        /// <param name="PswdEmptyHandler">Error handler in case the user password is null or empty</param>
        /// <param name="AccountInvalidHandler">Error handler in case the user email does not map to an account</param>
        /// <param name="LockoutHandler">Error handler in case the user account is locked out</param>
        /// <param name="LoginForbiddenHandler">Error handler in case the user name or password is invalid and the system cannot authenticate</param>
        /// <param name="PendingConfirmationHandler">Error handler in case user account has not yet been confirmed</param>
        /// <param name="DisabledHandler">Error handler in case user account is disabled</param>
        /// <param name="PswdExpiredHandler">Error handler in case user password has expired</param>
        /// <param name="LoginFailHandler">Error handler in case login attempt is invalid</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <param name="SuccessHandler">Success handler to trigger events after login</param>
        /// <param name="UserTokenHandler">Success handler to handle user token distribution</param>
        /// <returns></returns>
        override internal bool TryAuthenticateUser(
            string UserEmail,
            string UserPassword,
            Action<AuthResultCode> ResultHandler = null,
            Action<Guid> GeneralFailHandler = null,
            Func<User, bool> UserTokenHandler = null)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            UserEmail = UserEmail?.Trim();


            //validate user email
            if (UserEmail.IsEmpty())
            {
                ResultHandler?.Invoke(AuthResultCode.EmailEmpty);
                return false;
            }

            if (!UserEmail.IsValidEmail())
            {
                ResultHandler?.Invoke(AuthResultCode.EmailInvalid);
                return false;
            }


            //validate password
            if (UserPassword.IsEmpty())
            {
                ResultHandler?.Invoke(AuthResultCode.PswdEmpty);
                return false;
            }

            //validate password
            if (!Regex.IsMatch(UserPassword, CoreFactory.Singleton.Properties.PswdStrengthRegex))
            {
                ResultHandler?.Invoke(AuthResultCode.PswdInvalid);
                return false;
            }


            //get userAuth info (pswf info)
            UserAuthInfo userAuthInfo = null;
            try
            {
                userAuthInfo = GetUserAuthInfo_DB(UserEmail);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex);
                GeneralFailHandler?.Invoke(new Guid("B61DB416-F38E-495C-BFDF-317FDB7F8063"));
                return false;
            }

            //ensure account exists
            if (userAuthInfo == null)
            {
                ResultHandler?.Invoke(AuthResultCode.UserInvalid);
                return false;
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
                    ResultHandler?.Invoke(AuthResultCode.UserLocked);
                    return false;
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
                CoreFactory.Singleton.Logging.CreateErrorLog(ex);
                GeneralFailHandler?.Invoke(new Guid("EE2C6BB0-8E49-4B31-9CB1-8C246C3EFFD9"));
                return false;
            }
            if (validationStatus != PasswordValidationStatus.Success)
            {

                if (validationStatus == PasswordValidationStatus.PswdExpired)
                {
                    ResultHandler?.Invoke(AuthResultCode.PswdExpired);
                }
                else if (validationStatus == PasswordValidationStatus.InvalidUser)
                {
                    ResultHandler?.Invoke(AuthResultCode.UserInvalid);
                }
                else if (validationStatus == PasswordValidationStatus.HashMismatch)
                {
                    ResultHandler?.Invoke(AuthResultCode.CredentialsInvalid);
                }
                else
                {
                    GeneralFailHandler?.Invoke(new Guid("FC5D3DDB-A48B-49C9-922E-7A96CB53CA7E"));
                }

                return false;
            }




            //try to get the real user from CMS DB
            User cmsUser;
            try
            {
                cmsUser = UserReader.GetUser(userAuthInfo.UserID);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex);
                GeneralFailHandler?.Invoke(new Guid("EC7D4ACA-498F-49DA-994B-099292AA9BD8"));
                return false;
            }


            //validate CMS specific user
            if (cmsUser == null || cmsUser.ID == null)
            {
                ResultHandler?.Invoke(AuthResultCode.UserInvalid);
                return false;
            }


            //user not confirmed
            if (!cmsUser.IsConfirmed)
            {
                ResultHandler?.Invoke(AuthResultCode.PendingConfirmation);
                return false;
            }

            //user not approved
            if (!cmsUser.IsApproved)
            {
                ResultHandler?.Invoke(AuthResultCode.PendingApproval);
                return false;
            }

            //user disabled for some other reason
            //could be disabled by admin
            if (!cmsUser.IsEnabled)
            {
                ResultHandler?.Invoke(AuthResultCode.UserDisabled);
                return false;
            }



            var status = UserTokenHandler?.Invoke(cmsUser) ?? true;
            if (status)
            {
                //CoreFactory.Singleton.Logging.CreateDBActivityLog(ActivityLogType.UserLogin);
                ResultHandler?.Invoke(AuthResultCode.Success);
                return true;
            }
            else
            {
                return false;
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
