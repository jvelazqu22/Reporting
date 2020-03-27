using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TripDurationReport
{
    [Serializable]
    public class RawData : IRouteWhere, IRouteItineraryInformation, ICollapsible
    {
        public DateTime? Depdate { get; set; } = DateTime.MinValue;
        public DateTime? Arrdate { get; set; } = DateTime.MinValue;
        [ExchangeDate2]
        public DateTime? Invdate { get; set; } = DateTime.MinValue;
        [ExchangeDate1]
        public DateTime? Bookdate { get; set; } = DateTime.MinValue;
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;
        public DateTime? RArrDate { get; set; } = DateTime.MinValue;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public string Recloc { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public string Invoice { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Acctdesc { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal? Airchg { get; set; } = 0m;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal ActFare { get; set; } = 0m;
        public decimal MiscAmt { get; set; } = 0m;
        public string Airline { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public string fltno { get; set; } = string.Empty;
        public string DepTime { get; set; } = string.Empty;
        public string Arrtime { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public int Seg_Cntr { get; set; }
        public string Classcat { get; set; } = string.Empty;
        public int SeqNo { get; set; } = 0;
        public string DomIntl { get; set; } = string.Empty;
        public int Miles { get; set; } = 0;
        //[Currency(RecordType = RecordType.Air)]
        //public decimal? Actfare { get; set; }
        //[Currency(RecordType = RecordType.Air)]
        public string Farebase { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public string Seat { get; set; } = string.Empty;
        public string Stops { get; set; } = string.Empty;
        public string Segstatus { get; set; } = string.Empty;
        public string Tktdesig { get; set; } = string.Empty;
        public int? Rplusmin { get; set; } = 0;
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string OrigCarr { get; set; } = string.Empty;
        public int Days { get; set; } = 0;
        public string Itinerary { get; set; } = string.Empty;
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
