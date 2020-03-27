using System;

namespace Domain.Models.ReportPrograms.TopBottomTravelerAir
{
    public class MidData
    {
        public MidData()
        {
            Passlast = string.Empty;
            Passfrst = string.Empty;
            CtryCode = string.Empty;
            SourceAbbr = string.Empty;
            Airchg = 0;
            Offrdchg = 0;
            Invdate = DateTime.MinValue;
            Bookdate = DateTime.MinValue;
            Depdate = DateTime.MinValue;
            Plusmin = 0;
            LostAmt = 0;
            BkDays = 0;
        }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string CtryCode { get; set; }
        public string SourceAbbr { get; set; }
        public decimal Airchg { get; set; }
        public decimal Offrdchg { get; set; }
        public DateTime? Invdate { get; set; }
        public DateTime? Bookdate { get; set; }
        public DateTime? Depdate { get; set; }
        public int Plusmin { get; set; }
        public decimal LostAmt { get; set; }
        public int BkDays { get; set; }
    }
}
