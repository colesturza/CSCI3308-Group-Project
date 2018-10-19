using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.SmtpInterop;

namespace UHub.CoreLib.Config
{
    public sealed class CmsConfig_Mail
    {
        /// <summary>
        /// SMTP mail client to send "No Reply" emails
        /// <para></para>
        /// Default: null
        /// </summary>
        public SmtpConfig NoReplyMailConfig { get; set; } = null;
        /// <summary>
        /// The email address that Contact Us emails will be delivered to
        /// <para></para>
        /// Default: null
        /// </summary>
        public string ContactFormRecipientAddress { get; set; } = null;
    }
}
