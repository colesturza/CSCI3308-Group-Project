using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Logging
{
    [Flags]
    public enum EventLoggingMode
    {
        None = 0,

        LocalFile = 1,
        SystemEvents = 2,
        Database = 4,

        All = LocalFile | SystemEvents | Database
    }
}
