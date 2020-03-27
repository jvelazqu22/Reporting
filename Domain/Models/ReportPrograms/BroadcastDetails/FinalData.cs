using System;

namespace Domain.Models.ReportPrograms.BroadcastDetails
{
    public class FinalData
    {
        public int Usernumber { get; set; } = 0;
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
        public string Bcsndraddr { get; set; } = string.Empty;
        public string Bcsndrnam { get; set; } = string.Empty;
        public DateTime Nextrun { get; set; } = DateTime.MinValue;
        public string Setby { get; set; } = string.Empty;
        public string Holdrun { get; set; } = string.Empty;
        public int Reportdays { get; set; } = 0;
        public int Rptusernum { get; set; } = 0;
        public bool Usespcl { get; set; } = false;
        public bool Nodataoptn { get; set; } = false;
        public string Emailsubj { get; set; } = string.Empty;
        public int Savrptnum { get; set; } = 0;
        public int Udrkey { get; set; } = 0;
        public int Processkey { get; set; } = 0;
        public int Datetype { get; set; } = 0;
        public string Lastname { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Rpttype { get; set; } = string.Empty;
        public string Rptname { get; set; } = string.Empty;
        public string Dtypename { get; set; } = string.Empty;
        public string Otheruser { get; set; } = string.Empty;
        public string Schedlname { get; set; } = string.Empty;
        public string Freqname { get; set; } = string.Empty;
        public string Rundayinfo { get; set; } = string.Empty;
        public string Hldruninfo { get; set; } = string.Empty;
        public string Nodatainfo { get; set; } = string.Empty;
    }
}
