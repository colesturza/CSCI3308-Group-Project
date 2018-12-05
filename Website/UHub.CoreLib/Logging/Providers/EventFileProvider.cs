using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Logging.Interfaces;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Logging.Providers
{
    internal sealed class EventFileProvider : IEventLogProvider
    {
        internal EventFileProvider()
        {

        }



        public bool CreateLog(EventLogData EventData)
        {
            var prefix = $"{EventData.EventType.ToString()}_";



            try
            {
                string path1 = CoreFactory.Singleton.Properties.LogStoreDirectory;
                string path2 = prefix + DateTime.UtcNow.ToString("M-d-yyyy_hh-mm-ss-fff") + ".txt";

                string path = Path.Combine(path1, path2);


                File.AppendAllText(path, EventData.ToString());

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
