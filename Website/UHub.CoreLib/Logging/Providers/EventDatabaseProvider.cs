using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Logging.DataInterop;
using UHub.CoreLib.Logging.Interfaces;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Logging.Providers
{
    internal sealed class EventDatabaseProvider : IEventLogProvider
    {
        public bool CreateLog(EventLogData EventData)
        {

            return LocalEventWriter.TryWriteEvent(EventData);

        }

    }
}
