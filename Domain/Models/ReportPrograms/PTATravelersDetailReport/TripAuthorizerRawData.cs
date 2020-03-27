using System;
using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.PTATravelersDetailReport
{
    public class TripAuthorizerRawData : IRecKey
    {
        [ExchangeDate1]
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate2]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public DateTime? Depdate { get; set; } = DateTime.MinValue;
        [Currency(RecordType = RecordType.Air)]
        public decimal AirChg { get; set; } = 0m;
        public DateTime? Bookedgmt { get; set; } = DateTime.MinValue;
        public DateTime? StatusTime { get; set; } = DateTime.MinValue;
        public string Recloc { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public int TravAuthNo { get; set; } = 0;
        public string Rtvlcode { get; set; } = string.Empty;
        public string OutPolCods { get; set; } = string.Empty;
        public int AuthrzrNbr { get; set; } = 0;
        public string AuthStatus { get; set; } = string.Empty;
        public string DetlStatus { get; set; } = string.Empty;
        public string ApvReason { get; set; } = string.Empty;
        public int ApSequence { get; set; } = 0;
        public DateTime? DetStatTim { get; set; } = DateTime.MinValue;
        public int RecKey { get; set; } = 0;
        public int SGroupNbr { get; set; } = 0;
        public string CliAuthNbr { get; set; } = string.Empty;
        public int RecordNo { get; set; } = 0;
        public string Auth1Email { get; set; } = string.Empty;
        public string Authcomm { get; set; } = string.Empty;
    }
}
