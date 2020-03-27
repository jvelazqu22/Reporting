using System;

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.WtsFareSavings
{
    public class FinalData : IRecKey
    {
        public int RecKey { get; set; } = 0;
        public string Invoice { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Acctdesc { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public DateTime Invdate { get; set; } = DateTime.MinValue;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Valcarr { get; set; } = string.Empty;
        public string Itinerary { get; set; } = string.Empty;
        public DateTime Depdate { get; set; } = DateTime.MinValue;
        public DateTime Bookdate { get; set; } = DateTime.MinValue;
        public int Plusmin { get; set; } = 0;
        public string ReasCode { get; set; } = string.Empty;
        public string ReasDesc { get; set; } = string.Empty;
        public bool Exchange { get; set; } = false;
        public string Origticket { get; set; } = string.Empty;
        public decimal Airchg { get; set; } = 0m;
        public decimal Offrdchg { get; set; } = 0m;
        public decimal Stndchg { get; set; } = 0m;
        public decimal Savings { get; set; } = 0m;
        public decimal LostAmt { get; set; } = 0m;
    }
}
