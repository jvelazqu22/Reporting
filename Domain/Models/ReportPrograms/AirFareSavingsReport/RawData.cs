using System;
using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.AirFareSavingsReport
{
    public class RawData : IRouteWhere
    {
        public int RecKey { get; set; } = 0;
        public int SeqNo { get; set; } = 0;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Reascode { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime? InvDate { get; set; } = DateTime.Now;
        public string Ticket { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        [ExchangeDate2]
        public DateTime? RDepDate { get; set; } = DateTime.Now;
        public string Airline { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal Basefare { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal Offrdchg { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal Stndchg { get; set; } = 0m;
        public string Savingcode { get; set; } = string.Empty;
        public string SourceAbbr { get; set; } = string.Empty;
        public int Plusmin { get; set; } = 0;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;

        public string DitCode { get; set; } = string.Empty;
        //public int SeqNo { get; set; }
        public DateTime? RArrDate { get; set; }
    }
}
