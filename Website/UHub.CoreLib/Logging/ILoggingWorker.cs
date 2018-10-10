using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Logging
{
    internal interface ILoggingWorker
    {
        /// <summary>
        /// Create success message
        /// </summary>
        /// <param name="Message"></param>
        bool CreateSuccessLog(string Message);

        /// <summary>
        /// Create information message
        /// </summary>
        /// <param name="Message"></param>
        bool CreateMessageLog(string Message);


        /// <summary>
        /// Create warning message
        /// </summary>
        /// <param name="Message"></param>
        bool CreateWarningLog(string Message);


        /// <summary>
        /// Create failure message
        /// </summary>
        /// <param name="Message"></param>
        bool CreateFailureLog(string Message);


        /// <summary>
        /// Create error message
        /// </summary>
        /// <param name="Message"></param>
        bool CreateErrorLog(string Message);
    }
}
