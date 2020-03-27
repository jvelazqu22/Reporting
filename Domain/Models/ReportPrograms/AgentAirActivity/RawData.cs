using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.AgentAirActivity
{
    [Serializable]
    public class RawData : IRouteWhere, ICollapsible
    {
        [ExchangeDate1]
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate2]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public string Recloc { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public string Invoice { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string Agentid { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal Offrdchg { get; set; } = 0m;
        public DateTime? Depdate { get; set; } = DateTime.MinValue;
        public string Cardnum { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal Svcfee { get; set; } = 0m;
        public int Plusmin { get; set; } = 0;
        [Currency(RecordType = RecordType.Air)]
        public decimal Acommisn { get; set; } = 0m;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public decimal ActFare { get; set; } = 0;
        public decimal MiscAmt { get; set; } = 0;
        public string DepTime { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string fltno { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;
        public DateTime? RArrDate { get; set; } = DateTime.MinValue;
        public string Arrtime { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public int Seg_Cntr { get; set; } = 0;
        public string Classcat { get; set; } = string.Empty;
        public int SeqNo { get; set; } = 0;
        public string DomIntl { get; set; } = string.Empty;
        public int Miles { get; set; } = 0;
        public string Farebase { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public string Seat { get; set; } = string.Empty;
        public string Stops { get; set; } = string.Empty;
        public string Segstatus { get; set; } = string.Empty;
        public string Tktdesig { get; set; } = string.Empty;
        public int Rplusmin { get; set; } = 0;
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string OrigCarr { get; set; } = string.Empty;
        public decimal AirCo2 { get; set; }
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
