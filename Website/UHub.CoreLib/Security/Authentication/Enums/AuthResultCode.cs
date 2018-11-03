using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Security.Authentication
{
    public enum AuthResultCode
    {
        Success,
        UnknownError,

        UserInvalid,
        UserLocked,
        UserDisabled,
        CredentialsInvalid,

        EmailInvalid,
        EmailEmpty,
        EmailDuplicate,

        PswdInvalid,
        PswdEmpty,
        PswdNotChanged,
        PswdExpired,

        PendingConfirmation,
        PendingApproval,
    }
}
