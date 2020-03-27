using System;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.BroadcastStatus
{
    public class RawData : IRecKey
    {
        public int RecKey { get; set; }
        public DateTime Rundatetime { get; set; } = DateTime.MinValue;
        public bool Runokay { get; set; } = false;
        public string Agency { get; set; } = string.Empty;
        public int Usernumber { get; set; } = 0;
        public int Batchnum { get; set; } = 0;
        public DateTime Startdate { get; set; } = DateTime.MinValue;
        public DateTime Enddate { get; set; } = DateTime.MinValue;
        public string Emailaddr { get; set; } = string.Empty;
        public string Emailmsg { get; set; } = string.Empty;
        public string Emaillog { get; set; } = string.Empty;
        public string Bcsenderemail { get; set; } = string.Empty;
        public string Bcsendername { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Batchname { get; set; } = string.Empty;
    }
}
