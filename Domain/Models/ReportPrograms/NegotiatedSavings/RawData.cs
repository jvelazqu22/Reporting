using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.NegotiatedSavings
{
    public class RawData : IRouteWhere
    {
        public RawData()
        {
            BookDate = DateTime.MinValue;
            InvDate = DateTime.MinValue;
            Recloc = string.Empty;
            RecKey = 0;
            Invoice = string.Empty;
            Ticket = string.Empty;
            Acct = string.Empty;
            Break1 = string.Empty;
            Break2 = string.Empty;
            Break3 = string.Empty;
            Passlast = string.Empty;
            Passfrst = string.Empty;
            Reascode = string.Empty;
            Savingcode = string.Empty;
            Offrdchg = 0m;
            Airchg = 0m;
            Origin = string.Empty;
            Destinat = string.Empty;
            Airline = string.Empty;
            Mode = string.Empty;
            Connect = string.Empty;
            RDepDate = DateTime.MinValue;
            Fltno = string.Empty;
            Deptime = string.Empty;
            RArrDate = DateTime.MinValue;
            Arrtime = string.Empty;
            Class = string.Empty;
            Classcat = string.Empty;
            Actfare = 0m;
            Miscamt = 0m;
            Farebase = string.Empty;
            DitCode = string.Empty;
            Tktdesig = string.Empty;
            OrigOrigin = string.Empty;
            OrigDest = string.Empty;
            OrigCarr = string.Empty;
        }
        public DateTime? BookDate { get; set; }
        [ExchangeDate1]
        public DateTime? InvDate { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; }
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
        public string Savingcode { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Offrdchg { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public string Airline { get; set; }
        public string Mode { get; set; }
        public string Connect { get; set; }
        public DateTime? RDepDate { get; set; }
        public string Fltno { get; set; }
        public string Deptime { get; set; }
        public DateTime? RArrDate { get; set; }
        public string Arrtime { get; set; }
        public string Class { get; set; }
        public string ClassCode { get; set; }
        public string Classcat { get; set; }
        public int SeqNo { get; set; }
        public int? Miles { get; set; }
        public decimal Actfare { get; set; }
        public decimal Miscamt { get; set; }
        public string Farebase { get; set; }
        public string DitCode { get; set; }
        public string Seat { get; set; }
        public string Stops { get; set; }
        public string SegStatus { get; set; }
        public string Tktdesig { get; set; }
        public int Plusmin { get; set; }
        public string OrigOrigin { get; set; }
        public string OrigDest { get; set; }
        public string OrigCarr { get; set; }

    }
}
