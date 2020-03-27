using System;

using Domain.Interfaces;
using Domain.Helper;

namespace Domain.Models.ReportPrograms.ExceptAirReport
{
    public class RawData : IRecKey
    {
        public string Recloc { get; set; }
        public int RecKey { get; set; }
        public string Invoice { get; set; }
        public string Ticket { get; set; }
        public string Acct { get; set; }
        public string Break1 { get; set; }
        public string Break2 { get; set; }
        public string Break3 { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Reascode { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal? Airchg { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal? BaseFare { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal? Offrdchg { get; set; }
        public DateTime? Depdate { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public string Mode { get; set; }
        public string Connect { get; set; }
        [ExchangeDate1]
        public DateTime? Rdepdate { get; set; }
        public string Airline { get; set; }
        public string Fltno { get; set; }
        public string Class { get; set; }
        public int SeqNo { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
    }
}
