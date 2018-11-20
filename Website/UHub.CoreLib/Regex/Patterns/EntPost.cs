using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Regex.Patterns
{
    public static class EntPost
    {
        public const string NAME = @".{5,100}";
        public const string NAME_B = @"^.{5,100}$";

        public const string CONTENT= @".{4,2000}";
        public const string CONTENT_B = @"^.{4,2000}$";


    }
}
