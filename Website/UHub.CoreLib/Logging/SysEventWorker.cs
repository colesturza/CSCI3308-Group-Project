using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Logging
{
    internal class SysEventWorker : ILoggingWorker
    {
        //EVENT FOLDER
        private const string uhubEventSource = "UHUB-CMS";
        /// <summary>
        /// Event "folder"
        /// </summary>
        private string LogNameAdj { get; }
        /// <summary>
        /// Event source
        /// </summary>
        private string EventSourceAdj { get; }

        internal SysEventWorker(LoggingSource LogSrc, string SiteFriendlyName)
        {

            if (LogSrc == LoggingSource.Application)
            {
                LogNameAdj = "Application";
                EventSourceAdj = uhubEventSource + ": " + SiteFriendlyName;
            }
            else
            {
                LogNameAdj = uhubEventSource;
                EventSourceAdj = SiteFriendlyName;
            }

        }

        private bool GenerateLog(string message, EventLogEntryType EventType)
        {
            try
            {

                short category = 0;
                short eventID = 0;
                if (EventType == EventLogEntryType.SuccessAudit)
                {
                    category = 1;
                    eventID = 101;
                }
                else if (EventType == EventLogEntryType.Information)
                {
                    category = 2;
                    eventID = 201;
                }
                else if (EventType == EventLogEntryType.Warning)
                {
                    category = 3;
                    eventID = 301;
                }
                else if (EventType == EventLogEntryType.FailureAudit)
                {
                    category = 4;
                    eventID = 401;
                }
                else if (EventType == EventLogEntryType.Error)
                {
                    category = 5;
                    eventID = 501;
                }

                using (EventLog eventLog = new EventLog(LogNameAdj))
                {
                    eventLog.Source = EventSourceAdj;
                    eventLog.WriteEntry(message, EventType, eventID, category);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// Create success message
        /// </summary>
        /// <param name="message"></param>
        public bool CreateSuccessLog(string message)
        {
            return GenerateLog(message, EventLogEntryType.SuccessAudit);
        }


        /// <summary>
        /// Create information message
        /// </summary>
        /// <param name="message"></param>
        public bool CreateMessageLog(string message)
        {
            return GenerateLog(message, EventLogEntryType.Information);
        }


        /// <summary>
        /// Create warning message
        /// </summary>
        /// <param name="message"></param>
        public bool CreateWarningLog(string message)
        {
            return GenerateLog(message, EventLogEntryType.Warning);
        }


        /// <summary>
        /// Create failure message
        /// </summary>
        /// <param name="message"></param>
        public bool CreateFailureLog(string message)
        {
            return GenerateLog(message, EventLogEntryType.FailureAudit);
        }


        /// <summary>
        /// Create error message
        /// </summary>
        /// <param name="message"></param>
        public bool CreateErrorLog(string message)
        {
            return GenerateLog(message, EventLogEntryType.Error);
        }


    }
}
