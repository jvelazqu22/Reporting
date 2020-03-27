using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.MarketReport
{
    [Serializable]
    public class RawData : IRouteWhere, ICollapsible, IAirMileage
    {
        public string Acct { get; set; } = string.Empty;

        [Currency(RecordType = RecordType.Air)]
        public decimal ActFare { get; set; } = 0;

        [Currency(RecordType = RecordType.Air)]
        public decimal? AirChg { get; set; } = 0;

        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;

        public string ArrTime { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal? BaseFare { get; set; } = 0;

        [ExchangeDate2]
        public DateTime? BookDate { get; set; } = DateTime.MinValue;

        public string fltno { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string ClassCat { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public string DepTime { get; set; } = string.Empty;
        public string DestDesc { get; set; } = string.Empty;
        public string FareBase { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal? FareTax { get; set; } = 0;
        public string Flt_Mkt { get; set; } = string.Empty;
        public string Flt_Mkt2 { get; set; } = string.Empty;

        [ExchangeDate1]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;

        public string DomIntl { get; set; }
        public int Miles { get; set; } = 0;

        [Currency(RecordType = RecordType.Air)]
        public decimal MiscAmt { get; set; } = 0;

        public string OrgDesc { get; set; } = string.Empty;
        public string OrgDestEmp { get; set; } = string.Empty;
        public string OrigCarr { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string OrigOrigin { get; set; } = string.Empty;
        public int PlusMin { get; set; } = 0;
        public string RecLoc { get; set; } = string.Empty;
        public int RecordNo { get; set; } = 0;
        public int RplusMin { get; set; } = 0;
        public string Seat { get; set; } = string.Empty;
        public int Seg_Cntr { get; set; } = 0;
        public int SegNum { get; set; } = 0;
        public string SegStatus { get; set; } = string.Empty;
        public string Stops { get; set; } = string.Empty;
        public string TktDesig { get; set; } = string.Empty;
        public string ValcarMode { get; set; }
        public string ValCarr { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public DateTime? RArrDate { get; set; } = DateTime.MinValue;
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;
        public int RecKey { get; set; } = 0;
        public int SeqNo { get; set; } = 0;
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
