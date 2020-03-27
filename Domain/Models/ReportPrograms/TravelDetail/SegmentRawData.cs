using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravelDetail
{
    [Serializable]
    public class SegmentRawData : IRouteWhere, ICollapsible
    {
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;

        public DateTime? TripStart { get; set; }

        public string SourceAbbr { get; set; } = string.Empty;
        
        public string SegStatus { get; set; } = string.Empty;

        public string RecLoc { get; set; } = string.Empty;

        public string PassLast { get; set; } = string.Empty;

        public string PassFrst { get; set; } = string.Empty;

        [ExchangeDate1]
        public DateTime? InvDate { get; set; }

        public string fltno { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal MiscAmt { get; set; }
        public string Connect { get; set; } = string.Empty;

        public string ClassCode { get; set; } = string.Empty;

        public string Break3 { get; set; } = string.Empty;

        public string Break2 { get; set; } = string.Empty;

        public string Break1 { get; set; } = string.Empty;

        [Currency(RecordType = RecordType.Air)]
        public decimal ActFare { get; set; } = 0m;

        public string Acct { get; set; } = string.Empty;

        public string DitCode { get; set; } = string.Empty;
        public string DomIntl { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string DepTime { get; set; }
        public string Airline { get; set; } = string.Empty;
        [ExchangeDate2]
        public DateTime? RDepDate { get; set; }
        [ExchangeDate3]
        public DateTime? RArrDate { get; set; }
        public int Seg_Cntr { get; set; }
        public int Miles { get; set; }
        public int RecKey { get; set; } = 0;
        public int SeqNo { get; set; } = 0;
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
