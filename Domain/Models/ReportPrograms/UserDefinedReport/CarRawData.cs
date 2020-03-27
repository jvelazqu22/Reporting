using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.UserDefinedReport
{
    public class CarRawData : IRecKey, ICarbonCalculationsCar
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
            Invdate = DateTime.MinValue;
            Bookdate = DateTime.MinValue;
            Company = string.Empty;
            Autostat = string.Empty;
            Autocity = string.Empty;
            Rentdate = DateTime.MinValue;
            Dateback = DateTime.MinValue;
            CarType = string.Empty;
            Reascoda = string.Empty;
            Abookrat = 0m;
            Aexcprat = 0m;
            Milecost = string.Empty;
            Ratetype = string.Empty;
            Citycode = string.Empty;
            Carcode = string.Empty;
            Aconfirmno = string.Empty;
            Ainvbyagcy = false;
            SeqNo = 0;
            Compamt = 0m;
            CPlusMin = 0;
            Ccommisn = 0m;
            Days = 0;
            Numcars = 0;
            Carsvgcode = string.Empty;
            Carstdrate = 0m;
            Cartrantyp = string.Empty;
            Moneytype = string.Empty;
            Emailaddr = string.Empty;
            Gds = string.Empty;
            Carctrycod = string.Empty;
            Carvendcod = string.Empty;
            Carbktype = string.Empty;
            Smartctrc = 0;
            Comisablec = string.Empty;
            Carratetyp = string.Empty;
            Carsegstat = string.Empty;
            Carvoiddat = DateTime.MinValue;
            Trdtrxnumc = 0;
        }

        public int RecKey { get; set; }
        public string Agency { get; set; }
        public string Recloc { get; set; }
        public string Invoice { get; set; }

        [ExchangeDate2]
        public DateTime? Invdate { get; set; }

        public string Pseudocity { get; set; }
        public string Agentid { get; set; }

        [ExchangeDate3]
        public DateTime? Bookdate { get; set; }

        public string Company { get; set; }
        public string Autostat { get; set; }
        public string Autocity { get; set; }
        public DateTime? Rentdate { get; set; }

        [ExchangeDate1]
        public DateTime ReturnDate
        {
            get { return Rentdate.GetValueOrDefault().AddDays(Days); }
        }

        public DateTime Dateback { get; set; }
        public string CarType { get; set; }
        public string Reascoda { get; set; }

        [Currency(RecordType = RecordType.Car)]
        public decimal Abookrat { get; set; }

        [Currency(RecordType = RecordType.Car)]
        public decimal Aexcprat { get; set; }

        public string Milecost { get; set; }
        public string Ratetype { get; set; }
        public string Citycode { get; set; }
        public string Carcode { get; set; }
        public string Aconfirmno { get; set; }
        public bool Ainvbyagcy { get; set; }
        public int SeqNo { get; set; }

        [Currency(RecordType = RecordType.Car)]
        public decimal Compamt { get; set; }

        public int CPlusMin { get; set; }

        [Currency(RecordType = RecordType.Car)]
        public decimal Ccommisn { get; set; }

        public int Days { get; set; }
        public int Numcars { get; set; }
        public string Carsvgcode { get; set; }

        [Currency(RecordType = RecordType.Car)]
        public decimal Carstdrate { get; set; }

        public string Cartrantyp { get; set; }

        [CarCurrency]
        public string Moneytype { get; set; }

        public string Emailaddr { get; set; }
        public string Gds { get; set; }
        public string Carctrycod { get; set; }
        public string Carvendcod { get; set; }
        public string Carbktype { get; set; }
        public int Smartctrc { get; set; }
        public string Comisablec { get; set; }
        public string Carratetyp { get; set; }
        public string Carsegstat { get; set; }
        public DateTime? Carvoiddat { get; set; }
        public int Trdtrxnumc { get; set; }

        public decimal CarCo2 { get; set; }
    }
}
