using Domain.Helper;
using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.ConcurrentSegmentsBookedReport
{
    [Serializable]
    public class RawData : IRouteWhere, ICollapsible
    {
        [ExchangeDate1]
        public DateTime? InvDate { get; set; } = DateTime.Now;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public string RecLoc { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public string Ticket { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Bktool { get; set; } = string.Empty;
        public string Gds { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; } = 0m;
        public DateTime? Depdate { get; set; } = DateTime.Now;
        [ExchangeDate2]
        public DateTime? Bookdate { get; set; } = DateTime.MinValue;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string fltno { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Fltno { get; set; } = string.Empty;
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;
        public string DepTime { get; set; } = string.Empty;
        public DateTime? RArrDate { get; set; } = DateTime.MinValue;
        public string Arrtime { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public int Seg_Cntr { get; set; }
        public string Classcat { get; set; } = string.Empty;
        public string DomIntl { get; set; } = string.Empty;
        public int Miles { get; set; } = 0;
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
