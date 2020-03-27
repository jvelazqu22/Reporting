using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ClassOfServiceReport
{
    [Serializable]
    public class RawData : IRouteWhere, IFareByMileage, ICollapsible, IAirMileage
    {
        [ExchangeDate2]
        public DateTime? BookDate { get; set; } = DateTime.Now;
        [ExchangeDate1]
        public DateTime? InvDate { get; set; } = DateTime.Now;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public string Recloc { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal BaseFare { get; set; } = 0m;
        public string SourceAbbr { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public int SeqNo { get; set; }
        public DateTime? RArrDate { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string ClassCat { get; set; } = string.Empty;
        public string DepTime { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string fltno { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public int Seg_Cntr { get; set; }
        public string Mode { get; set; } = string.Empty;
        public DateTime? RDepDate { get; set; }
        //public DateTime Rarrdate { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal ActFare { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal MiscAmt { get; set; }
        public string DitCode { get; set; } = string.Empty;
        public string DomIntl { get; set; } = string.Empty;
        public int Miles { get; set; } = 0;
        public int Plusmin { get; set; } = 0;
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
