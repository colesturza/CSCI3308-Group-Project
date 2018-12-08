using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Logging.Interfaces;
using UHub.CoreLib.Logging.Util;

namespace UHub.CoreLib.Logging.Management
{
    // <summary>
    // Manage system logging | core init
    // </summary>
    public sealed partial class LoggingManager
    {

        
        public async Task CreatePageActionLogAsync(string Url)
        {
            var data = LoggingHelpers.GetUserClientData();

            await Task.Run(() => usageProviders.ForEach(x => x.CreatePageActionLog(Url, data)));
        }


        public async Task CreateApiActionLogAsync(string Url)
        {
            var data = LoggingHelpers.GetUserClientData();

            await Task.Run(() => usageProviders.ForEach(x => x.CreateApiActionLog(Url, data)));
        }


        public async Task CreateApiActionLogAsync(UsageLogType EventType)
        {
            var data = LoggingHelpers.GetUserClientData();

            await Task.Run(() => usageProviders.ForEach(x => x.CreateClientEventLog(EventType, data)));
        }


    }
}
