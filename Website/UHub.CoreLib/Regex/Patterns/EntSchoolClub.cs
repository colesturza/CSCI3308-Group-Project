using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Regex.Patterns
{
    public static class EntSchoolClub
    {

        public const string NAME = @"(([ \u00c0-\u01ffA-z'\-])+){3,200}";
        public const string NAME_B = @"^(([ \u00c0-\u01ffA-z'\-])+){3,200}$";

        public const string DESCRIPTION = @".{10, 2000}";
        public const string DESCRIPTION_B = @"^.{10, 2000}$";
    }
}
