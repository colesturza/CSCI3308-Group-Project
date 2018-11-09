﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Logging.Interfaces;

namespace UHub.CoreLib.Logging
{
    // <summary>
    // Manage system logging | core init
    // </summary>
    public sealed partial class LoggingManager
    {

        #region Site Analytics/Usage Logs
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


        public async Task CreateApiActionLogAsync(ActivityLogType EventType)
        {
            var data = LoggingHelpers.GetUserClientData();

            await Task.Run(() => usageProviders.ForEach(x => x.CreateClientEventLog(EventType, data)));
        }

        #endregion


    }
}