using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Logging.Interfaces;

namespace UHub.CoreLib.Logging.Management
{
    // <summary>
    // Manage system logging | core init
    // </summary>
    public sealed partial class LoggingManager
    {
        private List<ILocalLogProvider> localProviders;
        private List<IUsageLogProvider> usageProviders;


        internal LoggingManager()
        {
            localProviders = new List<ILocalLogProvider>();
            usageProviders = new List<IUsageLogProvider>();
        }

        internal void AddProvider(ILocalLogProvider LocalLogProvider)
        {
            localProviders.Add(LocalLogProvider);
        }
        internal void AddProvider(IUsageLogProvider UsageLogProvider)
        {
            usageProviders.Add(UsageLogProvider);
        }


        #region Site Analytics/Usage Logs

        #endregion


    }
}
