using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.PublishedSavings
{
    public class RawData : IRouteWhere
    {
        [ExchangeDate2]
        public DateTime? BookDate { get; set; } = DateTime.Now;
        [ExchangeDate1]
        public DateTime? InvDate { get; set; } = DateTime.Now;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public string Recloc { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public string Invoice { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Reascode { get; set; } = string.Empty;
        public string Savingcode { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal Stndchg { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; } = 0m;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public DateTime? RDepDate { get; set; } = DateTime.Now;
        public string Fltno { get; set; } = string.Empty;
        public string Deptime { get; set; } = string.Empty;
        public DateTime? RArrDate { get; set; } = DateTime.Now;
        public string Arrtime { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string Classcat { get; set; } = string.Empty;
        public int SeqNo { get; set; } = 0;
        public int? Miles { get; set; }
        public decimal? Actfare { get; set; } = 0m;
        public decimal? Miscamt { get; set; } = 0m;
        public string Farebase { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public string Seat { get; set; } = string.Empty;
        public string Stops { get; set; } = string.Empty;
        public string Segstatus { get; set; } = string.Empty;
        public string Tktdesig { get; set; } = string.Empty;
        public int Plusmin { get; set; }
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string OrigCarr { get; set; } = string.Empty;
    }
}
