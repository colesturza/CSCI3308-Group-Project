using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Security.Accounts
{
    //TODO: break into mutliple covariant enums
    //Allow consumers to properly handle codes by only seeing relevant outputs
    public enum AccountResultCode
    {
        Success,
        UnknownError,

        UserInvalid,

        NameInvalid,

        UsernameInvalid,
        UsernameDuplicate,

        EmailInvalid,
        EmailEmpty,
        EmailDuplicate,
        EmailDomainInvalid,

        MajorInvalid,

        PswdInvalid,
        PswdEmpty,
        PswdNotChanged,

        RecoveryContextInvalid,
        RecoveryContextDisabled,
        RecoveryContextExpired,
        RecoveryContextDestroyed,
        RecoveryKeyInvalid,


        LoginFailed,
    }
}
