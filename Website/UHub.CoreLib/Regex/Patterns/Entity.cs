using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Regex.Patterns
{
    public static class Entity
    {
        public const string ENT_NAME = @".{2,200}";
        public const string ENT_NAME_B = @"^.{2,200}$";
    }
}
