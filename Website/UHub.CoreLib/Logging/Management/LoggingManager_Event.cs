﻿using System;
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
            string uid = null;
            if (Data is Guid)
            {
                uid = Data.ToString();
            }

            var eventData = new EventLogData
            {
                EventType = EventType.Success,
                EventID = uid,
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
        public void CreateSuccessLog<T>(T Data, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Success,
                EventID = UID.ToString(),
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
        /// Create success message
        /// </summary>
        /// <param name="message"></param>
        public void CreateSuccessLog(string Message, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Success,
                EventID = UID.ToString(),
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
            string uid = null;
            if (Data is Guid)
            {
                uid = Data.ToString();
            }

            var eventData = new EventLogData
            {
                EventType = EventType.Information,
                EventID = uid,
                Content = Data.ToFormattedJSON(),
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
        public void CreateInfoLog<T>(T Data, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Information,
                EventID = UID.ToString(),
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
        /// Create information message
        /// </summary>
        /// <param name="message"></param>
        public void CreateInfoLog(string Message, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Information,
                EventID = UID.ToString(),
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
        public void CreateWarningLog<T>(T Data)
        {
            string uid = null;
            if (Data is Guid)
            {
                uid = Data.ToString();
            }

            var eventData = new EventLogData
            {
                EventType = EventType.Warning,
                EventID = uid,
                Content = Data.ToFormattedJSON(),
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
        public void CreateWarningLog<T>(T Data, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Warning,
                EventID = UID.ToString(),
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
        /// Create warning message
        /// </summary>
        /// <param name="message"></param>
        public void CreateWarningLog(string Message, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Warning,
                EventID = UID.ToString(),
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
            string uid = null;
            if (Data is Guid)
            {
                uid = Data.ToString();
            }

            var eventData = new EventLogData
            {
                EventType = EventType.Failure,
                EventID = uid,
                Content = Data.ToFormattedJSON(),
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
        public void CreateFailureLog<T>(T Data, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Failure,
                EventID = UID.ToString(),
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
        /// Create failure message
        /// </summary>
        /// <param name="message"></param>
        public void CreateFailureLog(string Message, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Failure,
                EventID = UID.ToString(),
                Content = Message,
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
            EventLogData eventData = null;
            string content = "";
            string uid = null;

            if (Data is Exception ex)
            {
                content = ex.ToString();
            }
            else if(Data is Guid)
            {
                uid = Data.ToString();
            }
            else
            {
                content = Data.ToFormattedJSON();
            }

            eventData = new EventLogData
            {
                EventType = EventType.Error,
                EventID = uid,
                Content = content,
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
        public void CreateErrorLog<T>(T Data, Guid UID)
        {
            EventLogData eventData = null;
            string content = "";

            if (Data is Exception ex)
            {
                content = ex.ToString();
            }
            else
            {
                content = Data.ToFormattedJSON();
            }

            eventData = new EventLogData
            {
                EventType = EventType.Error,
                EventID = UID.ToString(),
                Content = content,
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
        public void CreateErrorLog(string Message)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Error,
                EventID = null,
                Content = Message,
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
        public void CreateErrorLog(string Message, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Error,
                EventID = UID.ToString(),
                Content = Message,
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
            string uid = null;
            if (Data is Guid)
            {
                uid = Data.ToString();
            }

            var eventData = new EventLogData
            {
                EventType = EventType,
                EventID = uid,
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
        public void CreateEventLog<T>(T Data, EventType EventType, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType,
                EventID = UID.ToString(),
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
        public void CreateEventLog(string Message, EventType EventType, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType,
                EventID = UID.ToString(),
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
