using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Regex.Patterns
{
    public static class FileUpload
    {
        public const string CHUNK_ID = @"[0-9a-f]{12}\-([0-9]{1,20})\-([0-9]{1,20})";
        public const string CHUNK_ID_B = @"^[0-9a-f]{12}\-([0-9]{1,20})\-([0-9]{1,20})$";

        public const string FILE_NAME = @"[\w\-. ]{1,200}";
        public const string FILE_NAME_B = @"^[\w\-. ]{1,200}$";
    }
}
