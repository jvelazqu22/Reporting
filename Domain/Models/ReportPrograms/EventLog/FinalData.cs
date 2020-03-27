using System;

namespace Domain.Models.ReportPrograms.EventLog
{
    public class FinalData
    {
        public string Category { get; set; } = string.Empty;
        public string Eventdesc { get; set; } = string.Empty;
        public string Userid { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Ipaddress { get; set; } = string.Empty;
        public DateTime Eventdate { get; set; } = DateTime.MinValue;
        public string Targettype { get; set; } = string.Empty;
        public string Evnttarget { get; set; } = string.Empty;
        public string Targetname { get; set; } = string.Empty;
    }
}
