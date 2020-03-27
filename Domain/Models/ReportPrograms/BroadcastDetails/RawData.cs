using System;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.BroadcastDetails
{
    public class RawData : IRecKey
    {
        public int RecKey { get; set; }
        public int UserNumber { get; set; } = 0;
        public string Agency { get; set; } = string.Empty;
        public int Batchnum { get; set; } = 0;
        public string Batchname { get; set; } = string.Empty;
        public string Emailaddr { get; set; } = string.Empty;
        public string Acctlist { get; set; } = string.Empty;
        public int Prevhist { get; set; } = 0;
        public int Weekmonth { get; set; } = 0;
        public int Monthstart { get; set; } = 0;
        public int Monthrun { get; set; } = 0;
        public int Weekstart { get; set; } = 0;
        public int Weekrun { get; set; } = 0;
        public DateTime Nxtdstart { get; set; } = DateTime.MinValue;
        public DateTime Nxtdend { get; set; } = DateTime.MinValue;
        public DateTime Lastrun { get; set; } = DateTime.MinValue;
        public DateTime Lastdstart { get; set; } = DateTime.MinValue;
        public DateTime Lastdend { get; set; } = DateTime.MinValue;
        public bool Errflag { get; set; } = false;
        public bool Runspcl { get; set; } = false;
        public DateTime Spclstart { get; set; } = DateTime.MinValue;
        public DateTime Spclend { get; set; } = DateTime.MinValue;
        public int Pagebrklvl { get; set; } = 0;
        public string Titleacct { get; set; } = string.Empty;
        public string Bcsenderemail { get; set; } = string.Empty;
        public string Bcsendername { get; set; } = string.Empty;
        public DateTime Nextrun { get; set; } = DateTime.MinValue;
        public string Setby { get; set; } = string.Empty;
        public string Holdrun { get; set; } = string.Empty;
        public int Reportdays { get; set; } = 0;
        public int Rptusernum { get; set; } = 0;
        public bool Usespcl { get; set; } = false;
        public bool Nodataoptn { get; set; } = false;
        public string Emailsubj { get; set; } = string.Empty;
        public string Mailformat { get; set; } = string.Empty;
        public string Outputtype { get; set; } = string.Empty;
        public bool Displayuid { get; set; } = false;
        public string Outputdest { get; set; } = string.Empty;
        public int EProfileNo { get; set; } = 0;
        public string Emailccadr { get; set; } = string.Empty;
        public string LangCode { get; set; } = string.Empty;
        public int RunNewData { get; set; } = 0;
        public int Fystartmo { get; set; } = 0;
        public string Timezone { get; set; } = string.Empty;
        public double Gmtdiff { get; set; } = 0;
        public string Unilangcode { get; set; } = string.Empty;
        public bool Send_error_email { get; set; } = false;
        public int Savedrptnum { get; set; } = 0;
        public int Udrkey { get; set; } = 0;
        public int Processkey { get; set; } = 0;
        public int Datetype { get; set; } = 0;
        public string Lastname { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
    }
}
