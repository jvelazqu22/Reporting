using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.CO2CombinedSummaryReport
{
    [Serializable]
    public class RawData : IRecKey, ICarbonCalculations, ICollapsible, IAirMileage
    {
        [ExchangeDate1]
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate2]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        public DateTime? UseDate { get; set; } = DateTime.MinValue;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal AirChg { get; set; } = 0m;
        public int RecKey { get; set; } = 0;
        public int SeqNo { get; set; } = 0;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        //[Currency(RecordType = RecordType.Air)]
        public decimal ActFare { get; set; } = 0m;
        //[Currency(RecordType = RecordType.Air)]
        public decimal Basefare { get; set; } = 0m;
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;
        public DateTime? Rarrdate { get; set; } = DateTime.MinValue;
        public string ClassCode { get; set; } = string.Empty;
        public int Seg_Cntr { get; set; } = 0;
        public string fltno { get; set; } = string.Empty;
        public string DepTime { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string DomIntl { get; set; } = string.Empty;
        public int Miles { get; set; } = 0;
        //[Currency(RecordType = RecordType.Air)]
        public decimal MiscAmt { get; set; } = 0m;
        public string Farebase { get; set; } = string.Empty;
        public int Plusmin { get; set; } = 0;
        public string DitCode { get; set; } = string.Empty;
        public decimal AirCo2 { get; set; } = 0m;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
        public string HaulType { get; set; } = string.Empty;
        public string ClassCat { get; set; } = string.Empty;
        public string GroupField { get; set; } = string.Empty;

        public int Segments { get; set; } = 0;
    }
}
