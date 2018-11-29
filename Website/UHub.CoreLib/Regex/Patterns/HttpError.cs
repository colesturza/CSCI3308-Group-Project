using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Regex.Patterns
{
    public static class HttpError
    {
        public const string ERROR_PAGE = @"\.[A-z]+\/Error\/[1-9][0-9]{2}(\?.*)?";
        public const string ERROR_PAGE_B = @"^\.[A-z]+\/Error\/[1-9][0-9]{2}(\?.*)?$";
    }
}
