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
    // Manage system logging | local server events
    // </summary>
    public sealed partial class LoggingManager
    {


        /// <summary>
        /// Create success message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateSuccessLogAsync<T>(T Data)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Success,
                EventID = null,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateLogAsync(eventData);
        }
        /// <summary>
        /// Create success message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateSuccessLogAsync(string Message)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Success,
                EventID = null,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateLogAsync(eventData);
        }


        /// <summary>
        /// Create information message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateInfoLogAsync<T>(T Data)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Information,
                EventID = null,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateLogAsync(eventData);
        }
        /// <summary>
        /// Create information message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateInfoLogAsync(string Message)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Information,
                EventID = null,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateLogAsync(eventData);
        }


        /// <summary>
        /// Create warning message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateWarningLogAsync<T>(T Data)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Warning,
                EventID = null,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateLogAsync(eventData);
        }
        /// <summary>
        /// Create warning message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateWarningLogAsync(string Message)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Warning,
                EventID = null,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateLogAsync(eventData);
        }


        /// <summary>
        /// Create failure message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateFailureLogAsync<T>(T Data)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Failure,
                EventID = null,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateLogAsync(eventData);
        }
        /// <summary>
        /// Create failure message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateFailureLogAsync(string Message)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Failure,
                EventID = null,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateLogAsync(eventData);
        }


        /// <summary>
        /// Create error message
        /// </summary>
        /// <param name="ex"></param>
        public async Task CreateErrorLogAsync(Exception ex)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Error,
                EventID = null,
                Content = ex?.ToString() ?? "UNKNOWN EXCEPTION",
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateLogAsync(eventData);
        }
        public async Task CreateErrorLogAsync(string UID, Exception exInner)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Error,
                EventID = UID,
                Content = exInner?.ToString() ?? "UNKNOWN EXCEPTION",
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateLogAsync(eventData);
        }
        /// <summary>
        /// Create error message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateErrorLogAsync<T>(T Data)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Error,
                EventID = null,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateLogAsync(eventData);
        }
        /// <summary>
        /// Create error message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateErrorLogAsync<T>(string UID, T Data)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Error,
                EventID = UID,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateLogAsync(eventData);
        }




        //---------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------

        
        /// <summary>
        /// Create success message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateLogAsync<T>(T Data, EventType EventType)
        {
            var eventData = new EventLogData
            {
                EventType = EventType,
                EventID = null,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateLogAsync(eventData);
        }
        /// <summary>
        /// Create success message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateLogAsync<T>(string UID, T Data, EventType EventType)
        {
            var eventData = new EventLogData
            {
                EventType = EventType,
                EventID = UID,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateLogAsync(eventData);
        }
        /// <summary>
        /// Create success message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateLogAsync(string Message, EventType EventType)
        {
            var eventData = new EventLogData
            {
                EventType = EventType,
                EventID = null,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateLogAsync(eventData);
        }
        /// <summary>
        /// Create success message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateLogAsync(string UID, string Message, EventType EventType)
        {
            var eventData = new EventLogData
            {
                EventType = EventType,
                EventID = UID,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateLogAsync(eventData);
        }


        public async Task CreateLogAsync(EventLogData EventData)
        {
            await Task.Run(() => localProviders.ForEach(x => x.CreateLog(EventData)));
        }

    }
}
