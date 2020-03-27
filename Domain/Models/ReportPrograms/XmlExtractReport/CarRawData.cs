using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.XmlExtractReport
{

    public class CarRawData : IRecKey
    {
        public CarRawData()
        {
            RecKey = 0;
            Agency = string.Empty;
            Recloc = string.Empty;
            Invoice = string.Empty;
            Invdate = DateTime.MinValue;
            Pseudocity = string.Empty;
            Agentid = string.Empty;
            Company = string.Empty;
            Autostat = string.Empty;
            Autocity = string.Empty;
            Days = 0;
            Rentdate = DateTime.MinValue;
            Cartype = string.Empty;
            Carcode = string.Empty;
            Reascoda = string.Empty;
            Abookrat = 0m;
            Aexcprat = 0m;
            Milecost = string.Empty;
            Ratetype = string.Empty;
            Citycode = string.Empty;
            Numcars = 0;
            Confirmno = string.Empty;
            Compamt = 0m;
            Cplusmin = 0;
            Ccommisn = 0m;
            Cartrantyp = string.Empty;
            Invbyagcy = false;
            Moneytype = string.Empty;
            Emailaddr = string.Empty;
            Gds = string.Empty;
        }
        public int RecKey { get; set; }
        public string Agency { get; set; }
        public string Recloc { get; set; }
        public string Invoice { get; set; }
        public DateTime? Invdate { get; set; }
        public string Pseudocity { get; set; }
        public string Agentid { get; set; }
        public string Company { get; set; }
        public string Autostat { get; set; }
        public string Autocity { get; set; }
        public int Days { get; set; }
        public DateTime? Rentdate { get; set; }
        public string Cartype { get; set; }
        public string Carcode { get; set; }
        public string Reascoda { get; set; }
        public decimal Abookrat { get; set; }
        public decimal Aexcprat { get; set; }
        public string Milecost { get; set; }
        public string Ratetype { get; set; }
        public string Citycode { get; set; }
        public int Numcars { get; set; }
        public string Confirmno { get; set; }
        public decimal Compamt { get; set; }
        public int Cplusmin { get; set; }
        public decimal Ccommisn { get; set; }
        public string Cartrantyp { get; set; }
        public bool Invbyagcy { get; set; }
        public string Moneytype { get; set; }
        public string Emailaddr { get; set; }
        public string Gds { get; set; }
    }

}
