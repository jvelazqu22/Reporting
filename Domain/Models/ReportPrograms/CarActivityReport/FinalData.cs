using System;

namespace Domain.Models.ReportPrograms.CarActivityReport
{
    public class FinalData
    {
        public string Recloc { get; set; } = string.Empty;
        public int Cplusmin { get; set; } = 0;
        public string Acct { get; set; } = string.Empty;
        public string Acctdesc { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Autocity { get; set; } = string.Empty;
        public string Autostat { get; set; } = string.Empty;
        public DateTime Rentdate { get; set; } = DateTime.MinValue;
        public string Company { get; set; } = string.Empty;
        public string Cartype { get; set; } = string.Empty;
        public int Days { get; set; } = 0;
        public decimal Abookrat { get; set; } = 0m;
        public string Milecost { get; set; } = string.Empty;
        public string Ratetype { get; set; } = string.Empty;
        public decimal Weeknum { get; set; } = 0m;
    }
}
