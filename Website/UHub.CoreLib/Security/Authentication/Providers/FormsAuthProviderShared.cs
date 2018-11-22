using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Users.DataInterop;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;

namespace UHub.CoreLib.Security.Authentication.Providers
{
    internal sealed partial class FormsAuthProvider
    {

        private static class Shared
        {
            internal static AuthResultCode TryAuthenticate_ValidateFields(
                string UserEmail,
                string UserPassword)
            {

                //validate user email
                if (UserEmail.IsEmpty())
                {
                    return AuthResultCode.EmailEmpty;
                }

                if (!UserEmail.IsValidEmail())
                {
                    return AuthResultCode.EmailInvalid;
                }


                //validate password
                if (UserPassword.IsEmpty())
                {
                    return AuthResultCode.PswdEmpty;
                }

                //validate password
                if (!UserPassword.RgxIsMatch(CoreFactory.Singleton.Properties.PswdStrengthRegex))
                {
                    return AuthResultCode.PswdInvalid;
                }


                return AuthResultCode.Success;
            }


            internal static AuthResultCode TryAuthenticate_ValidateUserAccess(User CmsUser)
            {
                //validate CMS specific user
                if (CmsUser == null || CmsUser.ID == null)
                {
                    return AuthResultCode.UserInvalid;
                }


                //user not confirmed
                if (!CmsUser.IsConfirmed)
                {
                    return AuthResultCode.PendingConfirmation;
                }

                //user not approved
                if (!CmsUser.IsApproved)
                {
                    return AuthResultCode.PendingApproval;
                }

                //user disabled for some other reason
                //could be disabled by admin
                if (!CmsUser.IsEnabled)
                {
                    return AuthResultCode.UserDisabled;
                }

                return AuthResultCode.Success;
            }
        }




        private PasswordValidationStatus ValidatePassword(string UserEmail, string Password)
        {
            UserAuthData userAuthInfo = null;
            try
            {
                userAuthInfo = UserReader.TryGetUserAuthData(UserEmail);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("D1CBD896-CAB8-48AD-9DA6-DD2DE9CEA2A1", ex);
                return PasswordValidationStatus.UnknownError;
            }


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
