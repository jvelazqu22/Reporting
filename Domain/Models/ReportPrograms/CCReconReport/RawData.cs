using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.CCReconReport
{
    public class RawData : IRouteItineraryInformation, IRecKey
    {
        public string AirCurrTyp { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public int RecKey { get; set; }
        public string Cardnum { get; set; } = string.Empty;
        public string Valcarr { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string Trantype { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        public DateTime? Invdate { get; set; } = DateTime.Now;
        public DateTime? Trandate { get; set; } = DateTime.Now;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public decimal? Airchg { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public int SeqNo { get; set; }
        public string RecLoc { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public DateTime? Depdate { get; set; } = DateTime.Now;
        public DateTime? Arrdate { get; set; } = DateTime.Now;
    }
}
