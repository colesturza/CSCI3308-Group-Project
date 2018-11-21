using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Security.Authentication
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
    }
}
