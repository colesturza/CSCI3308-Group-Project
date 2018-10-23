using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Security.Accounts
{
    public enum AccountResultCode
    {
        Success,

        UserInvalid,
        UsernameDuplicate,

        EmailInvalid,
        EmailEmpty,
        EmailDuplicate,
        EmailDomainInvalid,

        MajorInvalid,

        PswdInvalid,
        PswdEmpty,
        PswdNotChanged,

        LoginFailed,
    }
}
