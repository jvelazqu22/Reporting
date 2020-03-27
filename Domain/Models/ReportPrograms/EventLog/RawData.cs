using System;

namespace Domain.Models.ReportPrograms.EventLog
{
    public class RawData
    {
        public string EventType { get; set; } = string.Empty;
        public string EventDesc { get; set; } = string.Empty;
        public int UserNumber { get; set; } = 0;
        public string UserID { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime DateStamp { get; set; } = DateTime.MinValue;
        public string TargetUserID { get; set; } = string.Empty;
        public int EventCode { get; set; } = 0;
        public string EventTarget { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
    }
}
