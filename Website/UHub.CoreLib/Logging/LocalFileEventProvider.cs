using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Logging.Interfaces;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Logging
{
    internal sealed class LocalFileEventProvider : ILocalLogProvider
    {
        

        internal LocalFileEventProvider()
        {

        }

        private bool GenerateLog(string message, EventLogEntryType EventType)
        {
            string prefix = "SystemLog_";
            if (EventType == EventLogEntryType.SuccessAudit)
            {
                prefix = "Success_";
            }
            else if (EventType == EventLogEntryType.Information)
            {
                prefix = "Information_";
            }
            else if (EventType == EventLogEntryType.Warning)
            {
                prefix = "Warning_";
            }
            else if (EventType == EventLogEntryType.FailureAudit)
            {
                prefix = "Failure_";
            }
            else if (EventType == EventLogEntryType.Error)
            {
                prefix = "Error_";
            }

            try
            {
                string path1 = CoreFactory.Singleton.Properties.LogStoreDirectory;
                string path2 = prefix + DateTime.UtcNow.ToString("M-d-yyyy_hh-mm-ss-fff") + ".txt";

                string path = Path.Combine(path1, path2);
                string outerMessage = DateTime.UtcNow.ToString() + Environment.NewLine + message + Environment.NewLine + Environment.NewLine;
                File.AppendAllText(path, outerMessage);

                return true;
            }
            catch
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
