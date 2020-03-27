using System;

using Domain.Helper;

namespace Domain.Models.ReportPrograms.AirFareSavingsReport
{
    public class FinalData
    {
        public FinalData()
        {
            AirCurrTyp = string.Empty;
            Reckey = 0;
            Seqno = 0;
            Ticket = string.Empty;
            Homectry = string.Empty;
            Acct = string.Empty;
            Acctdesc = string.Empty;
            Break1 = string.Empty;
            Break2 = string.Empty;
            Break3 = string.Empty;
            Invdate = DateTime.MinValue;
            Passlast = string.Empty;
            Passfrst = string.Empty;
            Origin = string.Empty;
            Orgdesc = string.Empty;
            Destinat = string.Empty;
            Destdesc = string.Empty;
            Rdepdate = DateTime.MinValue;
            Connect = string.Empty;
            Airline = string.Empty;
            Class = string.Empty;
            Airchg = 0m;
            Plusmin = 0;
            Reascode = string.Empty;
            Savingcode = string.Empty;
            Offrdchg = 0m;
            Stndchg = 0m;
            Savings = 0m;
            Negosvngs = 0m;
            Lostamt = 0m;
            Lostpct = 0m;
            Origacct = string.Empty;
            Sourceabbr = string.Empty;
            Udidlbl1 = string.Empty;
            Udidtext1 = string.Empty;
            Udidlbl2 = string.Empty;
            Udidtext2 = string.Empty;
            Udidlbl3 = string.Empty;
            Udidtext3 = string.Empty;
            Udidlbl4 = string.Empty;
            Udidtext4 = string.Empty;
            Udidlbl5 = string.Empty;
            Udidtext5 = string.Empty;
            Udidlbl6 = string.Empty;
            Udidtext6 = string.Empty;
            Udidlbl7 = string.Empty;
            Udidtext7 = string.Empty;
            Udidlbl8 = string.Empty;
            Udidtext8 = string.Empty;
            Udidlbl9 = string.Empty;
            Udidtext9 = string.Empty;
            Udidlbl10 = string.Empty;
            Udidtext10 = string.Empty;
            Invdatetxt = string.Empty;
            Rdepdattxt = string.Empty;
        }
        public int Reckey { get; set; }
        public int Seqno { get; set; }
        public string Ticket { get; set; }
        public string Homectry { get; set; }
        public string Acct { get; set; }
        public string Acctdesc { get; set; }
        public string Break1 { get; set; }
        public string Break2 { get; set; }
        public string Break3 { get; set; }
        [ExchangeDate1]
        public DateTime Invdate { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Origin { get; set; }
        public string Orgdesc { get; set; }
        public string Destinat { get; set; }
        public string Destdesc { get; set; }
        [ExchangeDate2]
        public DateTime Rdepdate { get; set; }
        public string Connect { get; set; }
        public string Airline { get; set; }
        public string Class { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; }
        public int Plusmin { get; set; }
        public string Reascode { get; set; }
        public string Savingcode { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Offrdchg { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Stndchg { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Savings { get; set; }
        public decimal Negosvngs { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Lostamt { get; set; }
        public decimal Lostpct { get; set; }
        public string Origacct { get; set; }
        public string Sourceabbr { get; set; }
        public string Udidlbl1 { get; set; }
        public string Udidtext1 { get; set; }
        public string Udidlbl2 { get; set; }
        public string Udidtext2 { get; set; }
        public string Udidlbl3 { get; set; }
        public string Udidtext3 { get; set; }
        public string Udidlbl4 { get; set; }
        public string Udidtext4 { get; set; }
        public string Udidlbl5 { get; set; }
        public string Udidtext5 { get; set; }
        public string Udidlbl6 { get; set; }
        public string Udidtext6 { get; set; }
        public string Udidlbl7 { get; set; }
        public string Udidtext7 { get; set; }
        public string Udidlbl8 { get; set; }
        public string Udidtext8 { get; set; }
        public string Udidlbl9 { get; set; }
        public string Udidtext9 { get; set; }
        public string Udidlbl10 { get; set; }
        public string Udidtext10 { get; set; }
        public string Invdatetxt { get; set; }
        public string Rdepdattxt { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; }
    }
}
