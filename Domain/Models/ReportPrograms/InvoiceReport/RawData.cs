using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.InvoiceReport
{
    public class RawData : IRecKey
    {
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public string Recloc { get; set; } = string.Empty;
        public string Pseudocity { get; set; } = string.Empty;
        public string Agentid { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime? Invdate { get; set; } = DateTime.MinValue;
        [ExchangeDate2]
        public DateTime? Bookdate { get; set; } = DateTime.MinValue;
        public string Trantype { get; set; } = string.Empty;
        public string Domintl { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Break4 { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; } = 0m;
        public string Credcard { get; set; } = string.Empty;
        public string Cardnum { get; set; } = string.Empty;
        public DateTime? Depdate { get; set; } = DateTime.MinValue;
        public string Valcarr { get; set; } = string.Empty;
        public bool Exchange { get; set; }
        public string Origticket { get; set; } = string.Empty;
        public decimal Tax1 { get; set; } = 0m;
        public decimal Tax2 { get; set; } = 0m;
        public decimal Tax3 { get; set; } = 0m;
        public decimal Tax4 { get; set; } = 0m;
        public string ValcarMode { get; set; } = string.Empty;
        public decimal SvcFee { get; set; } = 0m;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;
        public string Fltno { get; set; } = string.Empty;
        public string Deptime { get; set; } = string.Empty;
        public DateTime? RArrDate { get; set; } = DateTime.MinValue;
        public string Arrtime { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string Classcat { get; set; } = string.Empty;
        public int SeqNo { get; set; } = 0;
        public int Miles { get; set; } = 0;
        public decimal Actfare { get; set; } = 0m;
        public decimal Miscamt { get; set; } = 0m;
        public string Farebase { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public string Seat { get; set; } = string.Empty;
        public string Stops { get; set; } = string.Empty;
        public string Segstatus { get; set; } = string.Empty;
        public string Tktdesig { get; set; } = string.Empty;
        public int Rplusmin { get; set; } = 0;
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string OrigCarr { get; set; } = string.Empty;
    }
}
