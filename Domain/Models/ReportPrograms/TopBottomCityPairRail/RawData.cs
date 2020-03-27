using Domain.Helper;
using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.TopBottomCityPairRail
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
        public int RecKey { get; set; } = 0;
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal Faretax { get; set; } = 0m;
        public decimal Basefare { get; set; } = 0m;
        public string Orgdestemp { get; set; } = string.Empty;
        public int Plusmin { get; set; } = 0;
        public decimal NumTicks { get; set; } = 0;
        public int RecordNo { get; set; } = 0;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string fltno { get; set; } = string.Empty;
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;
        public string DepTime { get; set; } = string.Empty;
        public DateTime? RArrDate { get; set; } = DateTime.MinValue;
        public string Arrtime { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public int Seg_Cntr { get; set; }
        public string Classcat { get; set; } = string.Empty;
        public string DomIntl { get; set; } = string.Empty;
        public int Miles { get; set; } = 0;
        [Currency(RecordType = RecordType.Air)]
        public decimal ActFare { get; set; } = 0m;

        public decimal MiscAmt { get; set; } = 0m;
        public string Connect { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public int SeqNo { get; set; } = 0;
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
