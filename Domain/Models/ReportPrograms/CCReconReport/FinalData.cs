using Domain.Helper;
using System;

namespace Domain.Models.ReportPrograms.CCReconReport
{
    public class FinalData
    {
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public string Grpcode { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Acctdesc { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Breakfld { get; set; } = string.Empty;
        public string BreaksCol { get; set; } = string.Empty;
        public string Cardnum { get; set; } = string.Empty;
        public string Airlinenbr { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string Airlndesc { get; set; } = string.Empty;
        public string Trantype { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime Trandate { get; set; } = DateTime.Now;
        public string Passname { get; set; } = string.Empty;
        public string Descript { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; }
        public decimal Cctransamt { get; set; }
        public string Ccrefnbr { get; set; } = string.Empty;
        public int Udidnbr1 { get; set; }
        public string Udidtext1 { get; set; } = string.Empty;
        public int Udidnbr2 { get; set; }
        public string Udidtext2 { get; set; } = string.Empty;
        public int Udidnbr3 { get; set; }
        public string Udidtext3 { get; set; } = string.Empty;
        public int Udidnbr4 { get; set; }
        public string Udidtext4 { get; set; } = string.Empty;
        public int Udidnbr5 { get; set; }
        public string Udidtext5 { get; set; } = string.Empty;
        public int Udidnbr6 { get; set; }
        public string Udidtext6 { get; set; } = string.Empty;
        public int Udidnbr7 { get; set; }
        public string Udidtext7 { get; set; } = string.Empty;
        public int Udidnbr8 { get; set; }
        public string Udidtext8 { get; set; } = string.Empty;
        public int Udidnbr9 { get; set; }
        public string Udidtext9 { get; set; } = string.Empty;
        public int Udidnbr10 { get; set; }
        public string Udidtext10 { get; set; } = string.Empty;

        public int RecKey { get; set; }
        public string Valcarr { get; set; }
        public string RecLoc { get; set; }
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string SfCardNum { get; set; }
        public DateTime Depdate { get; set; } = DateTime.Now;
        public DateTime Arrdate { get; set; } = DateTime.Now;
    }
}
