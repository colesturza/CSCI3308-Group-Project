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

namespace UHub.CoreLib.Security.Authentication.Providers.Forms
{
    internal sealed partial class FormsAuthProvider : AuthenticationProvider
    {

        private PasswordValidationStatus ValidatePassword(string UserEmail, string Password)
        {
            UserAuthData userAuthInfo = null;
            try
            {
                userAuthInfo = UserReader.TryGetUserAuthData(UserEmail);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, "D1CBD896-CAB8-48AD-9DA6-DD2DE9CEA2A1");
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
