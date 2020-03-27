using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.AirActivityByUdidReport
{
    public class RawData : IRecKey
    {
        public RawData()
        {
            BookDate = DateTime.Now;
            InvDate = DateTime.Now;
            AirCurrTyp = "";
            RecKey = 0;
            SeqNo = 0;
            Udidtext = "";
            Passlast = "";
            Passfrst = "";
            Ticket = "";
            Airchg = 0;
            Origin = "";
            Destinat = "";
            Connect = "";
            Depdate = DateTime.Now;
            Rdepdate = DateTime.Now;
            Airline = "";
            Fltno = "";
            Mode = "";
            Acct = "";
            Break1 = "";
            Break2 = "";
            Break3 = "";
            Trantype = "";
            OrigCarr = "";

        }
        [ExchangeDate2]
        public DateTime? BookDate { get; set; }
        [ExchangeDate1]
        public DateTime? InvDate { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; }
        public int RecKey { get; set; }
        public int SeqNo { get; set; }
        public string Udidtext { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Ticket { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal? Airchg { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public string Connect { get; set; }
        public DateTime? Depdate { get; set; }
        public DateTime? Rdepdate { get; set; }
        public string Airline { get; set; }
        public string Fltno { get; set; }
        public string Mode { get; set; }
        public string Acct { get; set; }
        public string Break1 { get; set; }
        public string Break2 { get; set; }
        public string Break3 { get; set; }
        public string Trantype { get; set; }
        public string OrigCarr { get; set; }
        public string ClassCode { get; set; }
    }
}
