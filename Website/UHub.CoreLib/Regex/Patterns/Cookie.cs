using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Regex.Patterns
{
    public static class Cookie
    {
        /// <summary>
        /// letters and numbers to match a regular domain
        /// </summary>
        public const string DOMAIN = @"[a-z0-9\.\-_]*";
    }
}
