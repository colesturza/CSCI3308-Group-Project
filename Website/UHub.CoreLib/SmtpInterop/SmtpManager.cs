using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.SmtpInterop
{
    /// <summary>
    /// CMS SMTP (email) manager to send messages from the site
    /// </summary>
    public static class SmtpManager
    {
        /// <summary>
        /// Attempt to send an email using the system configuration
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static bool TrySendMessage(SmtpMessage Message)
        {
            return TrySendMessage(Message, out _);
        }


        /// <summary>
        /// Attempt to send an email using the system configuration
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static bool TrySendMessage(SmtpMessage Message, out SmtpResultCode result)
        {
            if (!Message.Validate())
            {
                result = SmtpResultCode.ValidationError;
                return false;
            }

            var from = CoreFactory.Singleton.Properties.NoReplyMailConfig.FromAddress;
            var to = Message.Recipient;
            var subj = Message.Subject;

            using (SmtpClient client = CoreFactory.Singleton.Properties.NoReplyMailConfig.GetSmtpClient())
            {
                client.EnableSsl = true;

                using (MailMessage msgOut = new MailMessage(from, new MailAddress(to)))
                {
                    msgOut.Subject = subj;
                    msgOut.IsBodyHtml = true;
                    msgOut.Body = Message.GetMessage();

                    try
                    {
                        client.Send(msgOut);
                        result = SmtpResultCode.Success;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        result = SmtpResultCode.SendError;
                        CoreFactory.Singleton.Logging.CreateErrorLog(ex);
                        return false;
                    }
                }

            }
        }

    }
}
