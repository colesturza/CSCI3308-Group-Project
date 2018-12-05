using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Logging.Interfaces
{
    internal interface ILocalLogProvider
    {

        /// <summary>
        /// Create success message
        /// </summary>
        /// <param name="Message"></param>
        bool CreateLog(EventLogData EventData);

    }
}
