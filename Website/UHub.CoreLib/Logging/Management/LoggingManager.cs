using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Logging.Interfaces;
using UHub.CoreLib.Logging.Providers;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Logging.Management
{
    // <summary>
    // Manage system logging | core init
    // </summary>
    public sealed partial class LoggingManager
    {
        private List<IEventLogProvider> localProviders;
        private List<IUsageLogProvider> usageProviders;


        internal LoggingManager(in CoreProperties properties)
        {
            localProviders = new List<IEventLogProvider>();
            usageProviders = new List<IUsageLogProvider>();


            //EVENTS
            if ((properties.EventLogMode & EventLoggingMode.LocalFile) != 0)
            {
                var fileProvider = new EventFileProvider();
                AddProvider(fileProvider);
            }
            if ((properties.EventLogMode & EventLoggingMode.SystemEvents) != 0)
            {
                var logSrc = properties.LoggingSource;
                var fName = properties.SiteFriendlyName;
                var eventProvider = new EventLocalSysProvider(logSrc, fName);

                AddProvider(eventProvider);
            }
            if ((properties.EventLogMode & EventLoggingMode.Database) != 0)
            {
                var dbProvider = new EventDatabaseProvider();
                AddProvider(dbProvider);
            }


            //USAGE
            if ((properties.UsageLogMode & UsageLoggingMode.GoogleAnalytics) != 0)
            {
                var googleProvider = new UsageGAnalyticsProvider();

                AddProvider(googleProvider);
            }

        }



        public void AddProvider(IEventLogProvider LocalLogProvider)
        {
            localProviders.Add(LocalLogProvider);
        }
        private void AddProvider(IUsageLogProvider UsageLogProvider)
        {
            usageProviders.Add(UsageLogProvider);
        }


    }
}
