using System.Collections.Generic;
using System.Net.Mail;

namespace Domain.Helper
{
    public class EmailInformation
    {
        public EmailInformation()
        {
            SenderAddress = string.Empty;
            SenderName  = string.Empty;
            RecipientAddress = string.Empty;
            RecipientName = string.Empty;
            CCList  = new List<string>();
            BCCList = new List<string>();
            Subject  = string.Empty;
            Message  = string.Empty;
            ContentType = string.Empty;
            IsBodyHtml = false;
        }
        public string SenderAddress { get; set; }
        public string SenderName { get; set; }
        public string RecipientAddress { get; set; }
        public string RecipientName { get; set; }
        public IList<string> CCList { get; set; }
        public IList<string> BCCList { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string ContentType { get; set; }
        public bool IsBodyHtml { get; set; }
        public List<Attachment> Attachments { get; set; } = null;
        
    }
}
