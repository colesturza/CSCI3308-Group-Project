using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Logging
{
    [Flags]
    public enum LocalLoggingMode
    {
        None = 0,

        LocalFile = 1,
        SystemEvents = 2,

        All = LocalFile | SystemEvents
    }
}
