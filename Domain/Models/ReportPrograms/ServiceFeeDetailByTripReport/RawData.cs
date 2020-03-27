using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.ServiceFeeDetailByTripReport
{
    public class RawData : IRecKey
    {
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        public string FeeCurrTyp { get; set; } = string.Empty;
        public string AirCurrTyp { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public int SfRecordNo { get; set; } = 0;
        public string Recloc { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public decimal? Airchg { get; set; } = 0m;
        public DateTime? Depdate { get; set; } = DateTime.MinValue;
        public DateTime? Invdate { get; set; } = DateTime.MinValue;
        public DateTime? Trandate { get; set; } = DateTime.MinValue;
        public decimal Svcfee { get; set; } = 0m;
        public string Descript { get; set; } = string.Empty;
    }
}
