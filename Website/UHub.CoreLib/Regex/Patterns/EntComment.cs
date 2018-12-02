using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Regex.Patterns
{
    public static class EntComment
    {
        public const string CONTENT = ".{1,1000}";
        public const string CONTENT_B = "^.{1,1000}$";

    }
}
