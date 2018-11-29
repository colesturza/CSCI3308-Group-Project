﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Regex.Patterns
{
    public static class Util
    {
        public const string STATE = @"[a-zA-Z ]{3,20}";
        public const string STATE_B = @"^[a-zA-Z ]{3,20}$";

        public const string CITY = @"(([A-z\u0080-\u024F]+(?:. |-| |'))*[A-z\u0080-\u024F])*";
        public const string CITY_B = @"^(([A-z\u0080-\u024F]+(?:. |-| |'))*[A-z\u0080-\u024F])$";


        public const string ZIP = @"[0-9]{5}(?:-[0-9]{4})?";
        public const string ZIP_B = @"^[0-9]{5}(?:-[0-9]{4})?$";
    }
}
