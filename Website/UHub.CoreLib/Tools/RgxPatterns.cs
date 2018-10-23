using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools
{
    /// <summary>
    /// Set of common regex patterns used throughout the sourcecode
    /// Does not include start/end bindings (eg, ^$) due to differing implementations
    /// </summary>
    internal static class RgxPatterns
    {
        internal static class Config
        {
            internal const string INTERNAL_URL = @"\~\/[a-zA-Z0-9\.\-_\/]*";
        }

        internal static class Cookie
        {
            /// <summary>
            /// letters and numbers to match a regular domain
            /// </summary>
            internal const string DOMAIN = @"[a-z0-9\.\-_]*";
        }

        internal static class User
        {
            internal const string USERNAME = @"\S{3,50}";
            internal const string EMAIL = @".{3,250}";
            internal const string NAME = @"(([ \u00c0-\u01ffA-z'\-])+){2,200}";
            internal const string REF_UID = @"[a-f0-9]{64}";
            internal const string PHONE = @"([0-1][ .-])?((\([0-9]{3}\)[ .-]?)|([0-9]{3}[ .-]?))([0-9]{3}[ .-]?)([0-9]{4})";
            internal const string MAJOR = @".{2,200}";
            internal const string YEAR = @".{1,50}";
            internal const string COMPANY = @".{0,100}";
            internal const string JOB_TITLE = @".{0,100}";
        }

        internal static class Entity
        {
            internal const string ENT_NAME = @".{2,200}";
        }

        internal static class Util
        {
            internal const string STATE = @"[A-z]*";
            internal const string CITY = @"(([A-z\u0080-\u024F]+(?:. |-| |'))*[A-z\u0080-\u024F])*";
            internal const string ZIP = @"[0-9]{5}(?:-[0-9]{4})?";
        }

        internal static class HttpError
        {
            internal const string ERROR_PAGE = @"\.[A-z]+\/Error\/[1-9][0-9]{2}(\?.*)?";

        }

        internal static class FileUpload
        {
            internal const string CHUNK_ID = @"[0-9a-f]{12}\-([0-9]{1,20})\-([0-9]{1,20})";
            internal const string FILE_NAME = @"[\w\-. ]{1,200}";
        }
    }
}
