using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Extensions;

namespace UHub.CoreLib.SmtpInterop
{
    /// <summary>
    /// SMTP (email) message to base
    /// </summary>
    public abstract class SmtpMessage
    {
        /// <summary>
        /// Message subject
        /// </summary>
        internal string Subject { get; }
        /// <summary>
        /// Message recipient
        /// </summary>
        internal string Recipient { get; }

        public SmtpMessage(string Subject, string Recipient)
        {
            this.Subject = Subject;
            this.Recipient = Recipient;
        }

        /// <summary>
        /// Render full message from email template and supplied arguments
        /// </summary>
        /// <returns></returns>
        internal string GetMessage()
        {
            if(this.Validate() && this.ValidateInner())
            {
                return this.RenderMessage();
            }
            else
            {
                throw new UHub.CoreLib.ErrorHandling.Exceptions.ConfigurationException("Message cannot be compiled due to invalid arguments");
            }
        }

        /// <summary>
        /// Validate base email arguments
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            if(!Recipient.IsValidEmail())
            {
                throw new ArgumentException("Recipient email address is invalid");
            }

            return true;
        }

        /// <summary>
        /// Validate child email arguments
        /// </summary>
        /// <returns></returns>
        protected abstract bool ValidateInner();

        /// <summary>
        /// Render true message
        /// </summary>
        /// <returns></returns>
        protected abstract string RenderMessage();
    }
}
