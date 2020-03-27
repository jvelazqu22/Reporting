using System;

namespace Domain.Models.ReportPrograms.ExceptCarReport
{
    public class FinalData
    {
        public int Reckey { get; set; }
        public string Recloc { get; set; }
        public string Acct { get; set; }
        public string Acctdesc { get; set; }
        public string Break1 { get; set; }
        public string Break2 { get; set; }
        public string Break3 { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Reascoda { get; set; }
        public string Reasdesc { get; set; }
        public DateTime Rentdate { get; set; }
        public string Company { get; set; }
        public string Autocity { get; set; }
        public string Autostat { get; set; }
        public int Days { get; set; }
        public string Cartype { get; set; }
        public string Ctypedesc { get; set; }
        public int Cplusmin { get; set; }
        public decimal Abookrat { get; set; }
        public decimal Aexcprat { get; set; }
        public string Cryspgbrk { get; set; }
    }
}
