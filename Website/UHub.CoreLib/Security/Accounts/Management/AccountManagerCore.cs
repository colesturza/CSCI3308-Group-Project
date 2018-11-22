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
using RgxPtrn = UHub.CoreLib.Regex.Patterns;
using System.Text.RegularExpressions;

namespace UHub.CoreLib.Security.Accounts.Management
{
    public sealed partial class AccountManager
    {

        private const short EMAIL_MIN_LEN = 3;
        private const short EMAIL_MAX_LEN = 250;
        private const short SALT_LEN = 50;
        private const short USER_VERSION_LEN = 10;
        private const short R_KEY_LEN = 20;


    }
}
