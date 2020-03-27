using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TripChangesAir
{
    public class RawData : IRouteWhere
    {
               
        [ExchangeDate1]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;

        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;

        public int RecKey { get; set; } = 0;

        public string Acct { get; set; } = string.Empty;

        public string Passlast { get; set; } = string.Empty;

        public string Passfrst { get; set; } = string.Empty;

        public string Mtggrpnbr { get; set; } = string.Empty;

        public string Ticket { get; set; } = string.Empty;

        public string Recloc { get; set; } = string.Empty;

        [ExchangeDate2]
        public DateTime? Bookdate { get; set; } = DateTime.MinValue;

        public DateTime? Depdate { get; set; } = DateTime.MinValue;

        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; } = 0m;

        public int ChangeCode { get; set; } = 0;

        public string ChangeDesc { get; set; } = string.Empty;

        public DateTime? ChangStamp { get; set; } = DateTime.MinValue;

        public string ChangeFrom { get; set; } = string.Empty;

        public string ChangeTo { get; set; } = string.Empty;

        public string PriorItin { get; set; } = string.Empty;

        public string Origin { get; set; } = string.Empty;

        public string Destinat { get; set; } = string.Empty;

        public string Airline { get; set; } = string.Empty;

        public string Mode { get; set; } = string.Empty;

        public string Connect { get; set; } = string.Empty;

        public string Fltno { get; set; } = string.Empty;

        public string Deptime { get; set; } = string.Empty;

        public string Arrtime { get; set; } = string.Empty;

        public string Class { get; set; } = string.Empty;

        public string ClassCode { get; set; } = string.Empty;

        public string Classcat { get; set; } = string.Empty;

        public int SeqNo { get; set; } = 0;

        [ExchangeDate3]
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;

        public DateTime? RArrDate { get; set; } = DateTime.MinValue;

        public int Miles { get; set; } = 0;
        
        public decimal Actfare { get; set; } = 0m;

        public decimal Miscamt { get; set; } = 0m;

        public string Farebase { get; set; } = string.Empty;

        public string DitCode { get; set; } = string.Empty;

        public int Segnum { get; set; } = 0;

        public string Seat { get; set; } = string.Empty;

        public string Stops { get; set; } = string.Empty;

        public string Segstatus { get; set; } = string.Empty;

        public string Tktdesig { get; set; } = string.Empty;

        public int RPlusmin { get; set; } = 0;

        public string OrigOrigin { get; set; } = string.Empty;

        public string OrigDest { get; set; } = string.Empty;

        public string OrigCarr { get; set; } = string.Empty;

    }

}
