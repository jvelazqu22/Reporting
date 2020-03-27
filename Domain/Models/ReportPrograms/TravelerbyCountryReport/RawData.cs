using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravelerbyCountryReport
{
    [Serializable]
    public class RawData : ICollapsible, IRouteWhere
    {
        [ExchangeDate2]
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate1]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public int? Plusmin { get; set; } = 0;
        public int Miles { get; set; } = 0;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal ActFare { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal MiscAmt { get; set; } = 0m;
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
        public string Farebase { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public string Seat { get; set; } = string.Empty;
        public string Stops { get; set; } = string.Empty;
        public string SegStatus { get; set; } = string.Empty;
        public string Tktdesig { get; set; } = string.Empty;
        public int RPlusMin { get; set; } = 0;
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string OrigCarr { get; set; } = string.Empty;
        public int Days { get; set; } = 0;
        public bool OneWayTrip { get; set; } = false;
        public decimal AirCo2 { get; set; } = 0m;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
