﻿using System;
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
    public sealed partial class SmtpManager
    {
        internal SmtpManager() { }


        /// <summary>
        /// Attempt to send an email using the system configuration
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public SmtpResultCode TrySendMessage(SmtpMessage Message)
        {
            if (!Message.Validate())
            {
                return SmtpResultCode.ValidationError;
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
                        return SmtpResultCode.Success;
                    }
                    catch (Exception ex)
                    {
                        CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex);
                        return SmtpResultCode.SendError;
                    }
                }

            }
        }

    }
}
