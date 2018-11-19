using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysSec = System.Web.Security;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;

namespace UHub.CoreLib.Security.Accounts
{
    public sealed partial class AccountManager
    {

        private static class Shared
        {

            internal static void TryCreate_HandleAttrTrim(ref User NewUser)
            {
                NewUser.Year = NewUser.Year?.Trim();
                NewUser.Company = NewUser.Company?.Trim();
                NewUser.Email = NewUser.Email?.Trim();
            }


            internal static AcctPswdResultCode ValidateUserPswd(User NewUser)
            {
                //ensure pswd is populated
                if (NewUser.Password.IsEmpty())
                {
                    return AcctPswdResultCode.PswdEmpty;
                }
                //check for valid password
                if (!NewUser.Password.RgxIsMatch(CoreFactory.Singleton.Properties.PswdStrengthRegex))
                {
                    return AcctPswdResultCode.PswdInvalid;
                }

                return AcctPswdResultCode.Success;
            }

            internal static AcctPswdResultCode ValidateUserPswd(string UserPswd)
            {
                //ensure pswd is populated
                if (UserPswd.IsEmpty())
                {
                    return AcctPswdResultCode.PswdEmpty;
                }
                //check for valid password
                if (!UserPswd.RgxIsMatch(CoreFactory.Singleton.Properties.PswdStrengthRegex))
                {
                    return AcctPswdResultCode.PswdInvalid;
                }

                return AcctPswdResultCode.Success;
            }


            internal static AcctCreateResultCode TryCreate_ValidateUserAttributes(User NewUser)
            {
                //ensure email is populated
                if (NewUser.Email.IsEmpty())
                {
                    return AcctCreateResultCode.EmailEmpty;
                }
                //check for valid email length
                if (NewUser.Email.Length < EMAIL_MIN_LEN || NewUser.Email.Length > EMAIL_MAX_LEN)
                {
                    return AcctCreateResultCode.EmailInvalid;
                }
                //check for valid email format
                if (!NewUser.Email.IsValidEmail())
                {
                    return AcctCreateResultCode.EmailInvalid;
                }


                //check for valid username
                if (NewUser.Username.RgxIsMatch(RgxPatterns.User.USERNAME_B))
                {
                    return AcctCreateResultCode.UsernameInvalid;
                }


                //check for invalid user name
                if (NewUser.Name.RgxIsMatch(RgxPatterns.User.NAME_B))
                {
                    return AcctCreateResultCode.NameInvalid;
                }

                //TODO: finalize attr validation


                return AcctCreateResultCode.Success;
            }


            internal static void TryCreate_HandleUserDefaults(ref User NewUser)
            {

                //set property constants
                bool isConfirmed = CoreFactory.Singleton.Properties.AutoConfirmNewAccounts;
                bool isApproved = CoreFactory.Singleton.Properties.AutoApproveNewAccounts;
                string userVersion = SysSec.Membership.GeneratePassword(USER_VERSION_LEN, 0);
                //sterilize for token processing
                userVersion = userVersion.Replace('|', '0');


                NewUser.IsConfirmed = isConfirmed;
                NewUser.IsApproved = isApproved;
                NewUser.Version = userVersion;

            }

        }
    }
}
