using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.AirTopBottomSegment
{
    [Serializable]
    public class RawData : IRouteWhere, ICollapsible
    {
        public int RecKey { get; set; } = 0;
        public short Plusmin { get; set; } = 0;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime? RDepDate { get; set; } = DateTime.Now;
        public string fltno { get; set; } = string.Empty;
        public string DepTime { get; set; } = string.Empty;
        [ExchangeDate2]
        public DateTime? RArrDate { get; set; } = DateTime.Now;
        public string Arrtime { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public int Seg_Cntr { get; set; }
        public string Classcat { get; set; } = string.Empty;
        public int SeqNo { get; set; } = 0;
        public string DomIntl { get; set; }
        public int Miles { get; set; } = 0;
        [Currency(RecordType = RecordType.Air)]
        public decimal ActFare { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal MiscAmt { get; set; } = 0m;
        public string Farebase { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public string seat { get; set; } = string.Empty;
        public string stops { get; set; } = string.Empty;
        public string segstatus { get; set; } = string.Empty;
        public string Tktdesig { get; set; } = string.Empty;
        public int Rplusmin { get; set; } = 0;
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string OrigCarr { get; set; } = string.Empty;
        [AirCurrency]
        public string AirCurrTyp { get; set; }
        public decimal AirCo2 { get; set; }
        public decimal AltCarCo2 { get; set; }
        public decimal AltRailCo2 { get; set; }
    }
}
