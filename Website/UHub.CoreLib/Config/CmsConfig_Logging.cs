﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Logging;

namespace UHub.CoreLib.Config
{
    public sealed class CmsConfig_Logging
    {
        /// <summary>
        /// Logging target mode.  Defines drop zone for log messages
        /// <para></para>
        /// Default: LocalFile
        /// </summary>
        public LocalLoggingMode LocalLogMode { get; set; } = LocalLoggingMode.LocalFile;
        /// <summary>
        /// Name of log folder if using SystemEvents
        /// <para></para>
        /// Default: Application
        /// </summary>
        public LoggingSource LoggingSource { get; set; } = LoggingSource.Application;

        public UsageLoggingMode UsageLogMode { get; set; } = UsageLoggingMode.None;
        /// <summary>
        /// Key for google analytics tracking
        /// </summary>
        public string GoogleAnalyticsKey { get; set; }
    }
}
