using System;

namespace Domain.Models.ReportPrograms.AgentAirActivity
{
    public class FinalData
    {
        public int RecKey { get; set; } = 0;
        public string Recloc { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public int Seqno { get; set; } = 0;
        public string Agentid { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Orgdesc { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Destdesc { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public DateTime Depdate { get; set; } = DateTime.MinValue;
        public string Cardnum { get; set; } = string.Empty;
        public DateTime Rdepdate { get; set; } = DateTime.MinValue;
        public string Airline { get; set; } = string.Empty;
        public string fltno { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public decimal Airchg { get; set; } = 0m;
        public decimal Offrdchg { get; set; } = 0m;
        public decimal Svcfee { get; set; } = 0m;
        public decimal Acommisn { get; set; } = 0m;
        public int Plusmin { get; set; } = 0;
    }
}
