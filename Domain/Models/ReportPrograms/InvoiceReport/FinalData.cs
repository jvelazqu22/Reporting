using System;

namespace Domain.Models.ReportPrograms.InvoiceReport
{
    public class FinalData
    {
        public string Rectype { get; set; } = string.Empty;
        public string Acctname { get; set; } = string.Empty;
        public string Brklvl1 { get; set; } = string.Empty;
        public string Brklvl2 { get; set; } = string.Empty;
        public string Brklvl3 { get; set; } = string.Empty;
        public string Acctaddr1 { get; set; } = string.Empty;
        public string Acctaddr2 { get; set; } = string.Empty;
        public string Acctaddr3 { get; set; } = string.Empty;
        public string Acctaddr4 { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        public DateTime Invdate { get; set; } = DateTime.MinValue;
        public DateTime Bookeddate { get; set; } = DateTime.MinValue;
        public DateTime Activdate { get; set; } = DateTime.MinValue;
        public string Agentid { get; set; } = string.Empty;
        public string Cardnum { get; set; } = string.Empty;
        public string Recloc { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Valcarr { get; set; } = string.Empty;
        public decimal Airchg { get; set; } = 0m;
        public decimal Svcfee { get; set; } = 0m;
        public string Orgdesc { get; set; } = string.Empty;
        public string Destdesc { get; set; } = string.Empty;
        public string Alinedesc { get; set; } = string.Empty;
        public string Fltno { get; set; } = string.Empty;
        public string Deptime { get; set; } = string.Empty;
        public string Arrtime { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Vendor { get; set; } = string.Empty;
        public string Vendaddr { get; set; } = string.Empty;
        public decimal Days { get; set; } = 0m;
        public decimal Rooms { get; set; } = 0m;
        public decimal Bookedrate { get; set; } = 0m;
        public string Ch_type { get; set; } = string.Empty;
        public string Hotphone { get; set; } = string.Empty;
        public bool Guaranteed { get; set; } = false;
        public decimal Reckey { get; set; } = 0m;
        public string Chargedesc { get; set; } = string.Empty;
        public decimal Charge { get; set; } = 0m;
        public string Exchinfo { get; set; } = string.Empty;
        public decimal Tax1 { get; set; } = 0m;
        public decimal Tax2 { get; set; } = 0m;
        public decimal Tax3 { get; set; } = 0m;
        public decimal Tax4 { get; set; } = 0m;
        public int Udidnbr1 { get; set; } = 0;
        public string Udidtext1 { get; set; } = string.Empty;
        public int Udidnbr2 { get; set; } = 0;
        public string Udidtext2 { get; set; } = string.Empty;
        public string Confirmno { get; set; } = string.Empty;
    }
}
