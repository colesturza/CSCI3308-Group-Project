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

        InvalidUser,

        EmailInvalid,
        EmailEmpty,
        EmailDuplicate,

        PswdInvalid,
        PswdEmpty,
        PswdNotChanged,

        LoginFailed,
    }
}
