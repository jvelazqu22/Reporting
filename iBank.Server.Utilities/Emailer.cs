using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Exceptions;
using Domain.Helper;
using Domain.Interfaces;

namespace iBank.Server.Utilities
{
    public class Emailer : IEmailer
    {
        private string _smtpServer = "";
        public string SmtpServer
        {
            get
            {
                if (string.IsNullOrEmpty(_smtpServer))
                {
                    _smtpServer = ConfigurationManager.AppSettings["SMTPHost"];
                }

                return _smtpServer;
            }
            set
            {
                _smtpServer = value;
            }
        }

        private int _smtpPort = 0;
        public int SmtpPort
        {
            get
            {
                if (_smtpPort == 0) _smtpPort = ConfigurationManager.AppSettings["SMTPPort"].TryIntParse(25);

                return _smtpPort;
            }
            set
            {
                _smtpPort = value;
            }
        }

        public Emailer()
        {
            SmtpServer = ConfigurationManager.AppSettings["SMTPHost"];
            SmtpPort = ConfigurationManager.AppSettings["SMTPPort"].TryIntParse(25);
        }
        
        public Emailer(string smtpServer, int smtpPort)
        {
            SmtpServer = smtpServer;
            SmtpPort = smtpPort;
        }

        public bool SendEmail(EmailInformation emailInfo)
        {
            using (var mailMsg = new MailMessage())
            {
                var apostrophe = '\'';
                mailMsg.From = new MailAddress(emailInfo.SenderAddress.RemoveTrailingChar(apostrophe), emailInfo.SenderName);

                var separators = new char[] { ',', ';' };
                var recipients = emailInfo.RecipientAddress.Split(separators);
                foreach (var recip in recipients)
                {
                    mailMsg.To.Add(recip.RemoveTrailingChar(apostrophe));
                }

                foreach (var cc in emailInfo.CCList.Where(x => !string.IsNullOrEmpty(x)))
                {
                    mailMsg.CC.Add(cc);
                }
                
                mailMsg.Subject = emailInfo.Subject;

                mailMsg.IsBodyHtml = emailInfo.IsBodyHtml;

                mailMsg.Body = emailInfo.Message;
                
                if (emailInfo.Attachments != null)
                {
                    foreach (var attach in emailInfo.Attachments)
                    {
                        mailMsg.Attachments.Add(attach);
                    }
                }

                var smtpClient = new SmtpClient(SmtpServer, SmtpPort) { Credentials = new NetworkCredential() };

                try
                {
                    smtpClient.Send(mailMsg);
                }
                catch (Exception ex)
                {
                    throw new SendEmailException(ex.Message);
                }

                return true;
            }
        }
    }
}
