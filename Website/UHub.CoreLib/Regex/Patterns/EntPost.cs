using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Regex.Patterns
{
    public static class EntPost
    {
        public const string NAME = @".{1,100}";
        public const string NAME_B = @"^.{1,100}$";

        public const string CONTENT= @".{10,10000}";
        public const string CONTENT_B = @"^.{10,10000}$";


    }
}
