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
        public void CreateSuccessLog<T>(T Data)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Success,
                EventID = null,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            CreateEventLog(eventData);
        }
        /// <summary>
        /// Create success message
        /// </summary>
        /// <param name="message"></param>
        public void CreateSuccessLog(string Message)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Success,
                EventID = null,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            CreateEventLog(eventData);
        }


        /// <summary>
        /// Create information message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void CreateInfoLog<T>(T Data)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Information,
                EventID = null,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            CreateEventLog(eventData);
        }
        /// <summary>
        /// Create information message
        /// </summary>
        /// <param name="message"></param>
        public void CreateInfoLog(string Message)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Information,
                EventID = null,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            CreateEventLog(eventData);
        }


        /// <summary>
        /// Create warning message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void  CreateWarningLog<T>(T Data)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Warning,
                EventID = null,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            CreateEventLog(eventData);
        }
        /// <summary>
        /// Create warning message
        /// </summary>
        /// <param name="message"></param>
        public void CreateWarningLog(string Message)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Warning,
                EventID = null,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            CreateEventLog(eventData);
        }


        /// <summary>
        /// Create failure message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void CreateFailureLog<T>(T Data)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Failure,
                EventID = null,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            CreateEventLog(eventData);
        }
        /// <summary>
        /// Create failure message
        /// </summary>
        /// <param name="message"></param>
        public void CreateFailureLog(string Message)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Failure,
                EventID = null,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            CreateEventLog(eventData);
        }


        /// <summary>
        /// Create error message
        /// </summary>
        /// <param name="ex"></param>
        public void CreateErrorLog(Exception ex)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Error,
                EventID = null,
                Content = ex?.ToString() ?? "UNKNOWN EXCEPTION",
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            CreateEventLog(eventData);
        }
        public void CreateErrorLog(string UID, Exception exInner)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Error,
                EventID = UID,
                Content = exInner?.ToString() ?? "UNKNOWN EXCEPTION",
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            CreateEventLog(eventData);
        }
        /// <summary>
        /// Create error message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void CreateErrorLog<T>(T Data)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Error,
                EventID = null,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            CreateEventLog(eventData);
        }
        /// <summary>
        /// Create error message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void CreateErrorLog<T>(string UID, T Data)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Error,
                EventID = UID,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            CreateEventLog(eventData);
        }




        //---------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------

        
        /// <summary>
        /// Create success message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void CreateEventLog<T>(T Data, EventType EventType)
        {
            var eventData = new EventLogData
            {
                EventType = EventType,
                EventID = null,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            CreateEventLog(eventData);
        }
        /// <summary>
        /// Create success message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void CreateEventLog<T>(string UID, T Data, EventType EventType)
        {
            var eventData = new EventLogData
            {
                EventType = EventType,
                EventID = UID,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            CreateEventLog(eventData);
        }
        /// <summary>
        /// Create success message
        /// </summary>
        /// <param name="message"></param>
        public void CreateEventLog(string Message, EventType EventType)
        {
            var eventData = new EventLogData
            {
                EventType = EventType,
                EventID = null,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            CreateEventLog(eventData);
        }
        /// <summary>
        /// Create success message
        /// </summary>
        /// <param name="message"></param>
        public void CreateEventLog(string UID, string Message, EventType EventType)
        {
            var eventData = new EventLogData
            {
                EventType = EventType,
                EventID = UID,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            CreateEventLog(eventData);
        }


        public void CreateEventLog(EventLogData EventData)
        {
            localProviders.ForEach(x => x.CreateLog(EventData));
        }

    }
}
