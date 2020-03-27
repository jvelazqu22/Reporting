using Domain.Helper;
using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.TopBottomCostCenter
{
    [Serializable]
    public class RawData : IRouteWhere, ICollapsible
    {
        public int RecKey { get; set; } = 0;
        [Currency(RecordType = RecordType.Air)]
        public decimal AirChg { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal OffrdChg { get; set; } = 0m;
        public string DitCode { get; set; } = string.Empty;
        public string DomIntl { get; set; } = string.Empty;
        public int Miles { get; set; } = 0;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public decimal ActFare { get; set; } = 0m;
        public decimal MiscAmt { get; set; } = 0m;
        public string Connect { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string DepTime { get; set; } = string.Empty;
        public string ArrTime { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string fltno { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public int Seg_Cntr { get; set; }
        public int SeqNo { get; set; } = 0;
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;
        public DateTime? RArrDate { get; set; } = DateTime.MinValue;

        [ExchangeDate1]
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate2]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public string GrpCol { get; set; } = string.Empty;
        public int NoHotel { get; set; } = 1;
        public int Plusmin { get; set; } = 0;
        public string Class { get; set; } = string.Empty;
        public string Classcat { get; set; } = string.Empty;
        public string Farebase { get; set; } = string.Empty;
        public string Seat { get; set; } = string.Empty;
        public string Stops { get; set; } = string.Empty;
        public string Segstatus { get; set; } = string.Empty;
        public string Tktdesig { get; set; } = string.Empty;
        public int Rplusmin { get; set; } = 0;
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string OrigCarr { get; set; } = string.Empty;
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
