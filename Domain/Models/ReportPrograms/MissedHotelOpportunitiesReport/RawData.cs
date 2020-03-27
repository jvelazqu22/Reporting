using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.MissedHotelOpportunitiesReport
{
    [Serializable]
    public class RawData : IRouteWhere, IRouteItineraryInformation, ICollapsible
    {
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        public string AirCurrTyp { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public string TranType { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        public DateTime? InvDate { get; set; } = DateTime.Now;
        public string Acct { get; set; } = string.Empty;
        public string AcctDesc { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public DateTime? TripStart { get; set; } = DateTime.MinValue;
        public DateTime? TripEnd { get; set; } = DateTime.MinValue;
        public string AgentId { get; set; } = string.Empty;
        public string SourceAbbr { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public decimal ActFare { get; set; }
        public decimal MiscAmt { get; set; }
        public string DepTime { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string fltno { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;
        public DateTime? RArrDate { get; set; } = DateTime.MinValue;
        public string Arrtime { get; set; }
        public string Class { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public int Seg_Cntr { get; set; }
        public string Classcat { get; set; } = string.Empty;
        public int SeqNo { get; set; }
        public string DomIntl { get; set; } = string.Empty;
        public int Miles { get; set; }
        public string Farebase { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public string Seat { get; set; } = string.Empty;
        public string Stops { get; set; } = string.Empty;
        public string SegStatus { get; set; } = string.Empty;
        public string Tktdesig { get; set; } = string.Empty;
        public int Plusmin { get; set; }
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string OrigCarr { get; set; } = string.Empty;
        public string Hotelbkd { get; set; } = string.Empty;
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
        public string RecLoc { get; set; } = string.Empty;
    }
}
