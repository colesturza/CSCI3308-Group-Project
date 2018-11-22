using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Logging.Interfaces;

namespace UHub.CoreLib.Logging
{
    // <summary>
    // Manage system logging | local server events
    // </summary>
    public sealed partial class LoggingManager
    {

        #region Local File/Event Logs
        /// <summary>
        /// Create success message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateSuccessLogAsync<T>(T data)
        {
            await CreateSuccessLogAsync(data.ToFormattedJSON());
        }
        /// <summary>
        /// Create success message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateSuccessLogAsync(string message)
        {
            await Task.Run(() => localProviders.ForEach(x => x.CreateSuccessLog(message)));
        }


        /// <summary>
        /// Create information message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateMessageLogAsync<T>(T data)
        {
            await CreateMessageLogAsync(data.ToFormattedJSON());
        }
        /// <summary>
        /// Create information message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateMessageLogAsync(string message)
        {
            await Task.Run(() => localProviders.ForEach(x => x.CreateMessageLog(message)));
        }


        /// <summary>
        /// Create warning message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateWarningLogAsync<T>(T data)
        {
            await CreateWarningLogAsync(data.ToFormattedJSON());
        }
        /// <summary>
        /// Create warning message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateWarningLogAsync(string message)
        {
            await Task.Run(() => localProviders.ForEach(x => x.CreateWarningLog(message)));
        }


        /// <summary>
        /// Create failure message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateFailureLogAsync<T>(T data)
        {
            await CreateFailureLogAsync(data.ToFormattedJSON());
        }
        /// <summary>
        /// Create failure message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateFailureLogAsync(string message)
        {
            await Task.Run(() => localProviders.ForEach(x => x.CreateFailureLog(message)));
        }


        /// <summary>
        /// Create error message
        /// </summary>
        /// <param name="ex"></param>
        public async Task CreateErrorLogAsync(Exception ex)
        {
            await CreateErrorLogAsync(ex?.ToString() ?? "UNKNOWN EXCEPTION");
        }
        public async Task CreateErrorLogAsync(string UID, Exception exInner)
        {
            Exception exOuter = new Exception(UID, exInner);

            await CreateErrorLogAsync(exInner?.ToString() ?? "UNKNOWN EXCEPTION");
        }
        /// <summary>
        /// Create error message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateErrorLogAsync<T>(T data)
        {
            await CreateErrorLogAsync(data.ToFormattedJSON());
        }
        /// <summary>
        /// Create error message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateErrorLogAsync(string message)
        {
            await Task.Run(() => localProviders.ForEach(x => x.CreateErrorLog(message)));
        }
        #endregion Local File/Event Logs


    }
}
