using System;

namespace Domain.Models.ReportPrograms.BroadcastStatus
{
    public class FinalData
    {
        public int Recnumber { get; set; } = 0;
        public DateTime Rundatetim { get; set; } = DateTime.MinValue;
        public bool Runokay { get; set; } = false;
        public string Agency { get; set; } = string.Empty;
        public int Usernumber { get; set; } = 0;
        public int Batchnum { get; set; } = 0;
        public DateTime Startdate { get; set; } = DateTime.MinValue;
        public DateTime Enddate { get; set; } = DateTime.MinValue;
        public string Emailaddr { get; set; } = string.Empty;
        public string Emailmsg { get; set; } = string.Empty;
        public string Emaillog { get; set; } = string.Empty;
        public string Bcsenderem { get; set; } = string.Empty;
        public string Bcsenderna { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Batchname { get; set; } = string.Empty;

    }
}
