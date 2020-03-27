using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.PassengersOnPlaneReport
{
    public class RawData : IRouteWhere
    {
        [ExchangeDate1]
        public DateTime? InvDate { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; }
        public int RecKey { get; set; }
        public string Recloc { get; set; }
        public string Acct { get; set; }
        public string Break1 { get; set; }
        public string Break2 { get; set; }
        public string Break3 { get; set; }
        [ExchangeDate3]
        public DateTime? Depdate { get; set; }
        [ExchangeDate2]
        public DateTime? Bookdate { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Pseudocity { get; set; }
        public string Domintl { get; set; }
        public string Ticket { get; set; }
        public string Agentid { get; set; }
        public string Trantype { get; set; }
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
        public string ClassCode { get; set; }
        public string Classcat { get; set; }

        public int SeqNo { get; set; }
        public int Miles { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal? Actfare { get; set; }
        public string Farebase { get; set; }
        public string DitCode { get; set; }
        public string Seat { get; set; }
        public string Stops { get; set; }
        public string Segstatus { get; set; }
        public string Tktdesig { get; set; }

        public int Rplusmin { get; set; }
        public string OrigOrigin { get; set; }
        public string OrigDest { get; set; }
        public string OrigCarr { get; set; }
    }
}
