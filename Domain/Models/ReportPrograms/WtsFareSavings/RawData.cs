using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.WtsFareSavings
{
    public class RawData : IRouteItineraryInformation
    {
        public int RecKey { get; set; } = 0;
        public int SeqNo { get; set; } = 0;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Valcarr { get; set; } = string.Empty;
        public string ValcarMode { get; set; } 
        public string ReasCode { get; set; } = string.Empty;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime? Bookdate { get; set; } = DateTime.MinValue;
        [ExchangeDate2]
        public DateTime? Invdate { get; set; } = DateTime.MinValue;
        [ExchangeDate3]
        public DateTime? Depdate { get; set; } = DateTime.MinValue;
        public string Ticket { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public DateTime? Rdepdate { get; set; } = DateTime.MinValue;
        public string Airline { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string OrigOrigin { get; set; } 
        public string OrigDest { get; set; } 
        public string Connect { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal Offrdchg { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal Stndchg { get; set; } = 0m;
        public int Plusmin { get; set; } = 0;
        public bool Exchange { get; set; } = false;
        public string Origticket { get; set; } = string.Empty;
    }
}
