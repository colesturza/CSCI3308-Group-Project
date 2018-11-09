using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Tools
{
    /// <summary>
    /// Set of common regex patterns used throughout the sourcecode
    /// Does not include start/end bindings (eg, ^$) due to differing implementations
    /// </summary>
    public static class RgxPatterns
    {
        public static class Config
        {
            public const string INTERNAL_URL = @"\~\/[a-zA-Z0-9\.\-_\/]*";
        }

        public static class Cookie
        {
            /// <summary>
            /// letters and numbers to match a regular domain
            /// </summary>
            public const string DOMAIN = @"[a-z0-9\.\-_]*";
        }

        public static class User
        {
            public const string USERNAME = @"\S{3,50}";
            public const string USERNAME_B = @"^\S{3,50}$";
            
            //ASSOC: 5F6FC523-2852-4C5A-91A0-3A3F05556594
            public const string PASSWORD = @".{8,150}";
            public const string PASSWORD_B = @"^.{8,150}$";

            public const string EMAIL = @".{3,250}";
            public const string EMAIL_B = @"^.{3,250}$";

            public const string NAME = @"(([ \u00c0-\u01ffA-z'\-])+){2,200}";
            public const string NAME_B = @"^(([ \u00c0-\u01ffA-z'\-])+){2,200}$";

            public const string REF_UID = @"[a-f0-9]{96}";
            public const string REF_UID_B = @"^[a-f0-9]{96}$";

            public const string PHONE = @"([0-1][ .-])?((\([0-9]{3}\)[ .-]?)|([0-9]{3}[ .-]?))([0-9]{3}[ .-]?)([0-9]{4})";
            public const string PHONE_B = @"^([0-1][ .-])?((\([0-9]{3}\)[ .-]?)|([0-9]{3}[ .-]?))([0-9]{3}[ .-]?)([0-9]{4})$";

            public const string MAJOR = @".{2,200}";
            public const string MAJOR_B = @"^.{2,200}$";

            public const string YEAR = @".{1,50}";
            public const string YEAR_B = @"^.{1,50}$";

            public const string COMPANY = @".{0,100}";
            public const string COMPANY_B = @"^.{0,100}$";

            public const string JOB_TITLE = @".{0,100}";
            public const string JOB_TITLE_B = @"^.{0,100}$";
        }

        public static class Entity
        {
            public const string ENT_NAME = @".{2,200}";
            public const string ENT_NAME_B = @"^.{2,200}$";
        }

        public static class Util
        {
            public const string STATE = @"[a-zA-Z ]{3,20}";
            public const string STATE_B = @"^[a-zA-Z ]{3,20}$";

            public const string CITY = @"(([A-z\u0080-\u024F]+(?:. |-| |'))*[A-z\u0080-\u024F])*";
            public const string CITY_B = @"^(([A-z\u0080-\u024F]+(?:. |-| |'))*[A-z\u0080-\u024F])$";


            public const string ZIP = @"[0-9]{5}(?:-[0-9]{4})?";
            public const string ZIP_B = @"^[0-9]{5}(?:-[0-9]{4})?$";
        }

        public static class HttpError
        {
            public const string ERROR_PAGE = @"\.[A-z]+\/Error\/[1-9][0-9]{2}(\?.*)?";
            public const string ERROR_PAGE_B = @"^\.[A-z]+\/Error\/[1-9][0-9]{2}(\?.*)?$";

        }

        public static class FileUpload
        {
            public const string CHUNK_ID = @"[0-9a-f]{12}\-([0-9]{1,20})\-([0-9]{1,20})";
            public const string CHUNK_ID_B = @"^[0-9a-f]{12}\-([0-9]{1,20})\-([0-9]{1,20})$";

            public const string FILE_NAME = @"[\w\-. ]{1,200}";
            public const string FILE_NAME_B = @"^[\w\-. ]{1,200}$";
        }
    }
}
