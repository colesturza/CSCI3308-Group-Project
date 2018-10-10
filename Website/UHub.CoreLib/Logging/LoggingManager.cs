using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Extensions;

namespace UHub.CoreLib.Logging
{
    // <summary>
    // Manage system logging | local server events
    // </summary>
    public sealed partial class LoggingManager
    {
        private ILoggingWorker logWorker { get; }


        internal LoggingManager(ILoggingWorker LogWorker)
        {
            logWorker = LogWorker;

        }


        #region Local File/Event Logs
        /// <summary>
        /// Create success message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void CreateSuccessLog<T>(T data)
        {
            CreateSuccessLog(data.ToFormattedJSON());
        }
        /// <summary>
        /// Create success message
        /// </summary>
        /// <param name="message"></param>
        public void CreateSuccessLog(string message)
        {
            logWorker.CreateSuccessLog(message);
        }


        /// <summary>
        /// Create information message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void CreateMessageLog<T>(T data)
        {
            CreateMessageLog(data.ToFormattedJSON());
        }
        /// <summary>
        /// Create information message
        /// </summary>
        /// <param name="message"></param>
        public void CreateMessageLog(string message)
        {
            logWorker.CreateMessageLog(message);
        }


        /// <summary>
        /// Create warning message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void CreateWarningLog<T>(T data)
        {
            CreateWarningLog(data.ToFormattedJSON());
        }
        /// <summary>
        /// Create warning message
        /// </summary>
        /// <param name="message"></param>
        public void CreateWarningLog(string message)
        {
            logWorker.CreateWarningLog(message);
        }


        /// <summary>
        /// Create failure message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void CreateFailureLog<T>(T data)
        {
            CreateFailureLog(data.ToFormattedJSON());
        }
        /// <summary>
        /// Create failure message
        /// </summary>
        /// <param name="message"></param>
        public void CreateFailureLog(string message)
        {
            logWorker.CreateFailureLog(message);
        }


        /// <summary>
        /// Create error message
        /// </summary>
        /// <param name="ex"></param>
        public void CreateErrorLog(Exception ex)
        {
            CreateErrorLog(ex?.ToString() ?? "UNKNOWN EXCEPTION");
        }
        /// <summary>
        /// Create error message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void CreateErrorLog<T>(T data)
        {
            CreateErrorLog(data.ToFormattedJSON());
        }
        /// <summary>
        /// Create error message
        /// </summary>
        /// <param name="message"></param>
        public void CreateErrorLog(string message)
        {
            logWorker.CreateErrorLog(message);
        }
        #endregion Local File/Event Logs


    }
}
