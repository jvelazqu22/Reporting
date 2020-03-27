using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.XmlExtractReport
{
    public class RawData : IRecKey, IXmlTripData, IXmlTravelerInfo
    {
        public RawData()
        {
            Agency = string.Empty;
            RecKey = 0;
            Recloc = string.Empty;
            Branch = string.Empty;
            Agentid = string.Empty;
            Pseudocity = string.Empty;
            Acct = string.Empty;
            Invoice = string.Empty;
            Invdate = DateTime.MinValue;
            Bookdate = DateTime.MinValue;
            Domintl = string.Empty;
            Ticket = string.Empty;
            Passlast = string.Empty;
            Passfrst = string.Empty;
            Break1 = string.Empty;
            Break2 = string.Empty;
            Break3 = string.Empty;
            Break4 = string.Empty;
            Airchg = 0m;
            Credcard = string.Empty;
            Cardnum = string.Empty;
            Arrdate = DateTime.MinValue;
            Mktfare = 0m;
            Valcarr = string.Empty;
            Depdate = DateTime.MinValue;
            Stndchg = 0m;
            Offrdchg = 0m;
            Reascode = string.Empty;
            Savingcode = string.Empty;
            Bktool = string.Empty;
            Bkagent = string.Empty;
            Tkagent = string.Empty;
            Tickettype = string.Empty;
            Faretax = 0m;
            Basefare = 0m;
            Corpacct = string.Empty;
            Sourceabbr = string.Empty;
            Tax4 = 0m;
            Tax3 = 0m;
            Tax2 = 0m;
            Tax1 = 0m;
            Origticket = string.Empty;
            Exchange = false;
            Moneytype = string.Empty;
            Iatanbr = string.Empty;
            Chgindcatr = string.Empty;
            Trantype = string.Empty;
            Plusmin = 0;
            Svcfee = 0m;
            Acommisn = 0m;
            Tourcode = string.Empty;
            Cancelcode = string.Empty;
            Phone = string.Empty;
            Lastupdate = DateTime.MinValue;
            Gds = string.Empty;
            Changedby = string.Empty;
            Changstamp = DateTime.MinValue;
            Parsestamp = DateTime.MinValue;
            Emailaddr = string.Empty;

            RecLoc6 = string.Empty;
            PassNbr = string.Empty;
            PnrPaxCnt = 1;
        }
        public string Agency { get; set; }
        public int RecKey { get; set; }
        public string Recloc { get; set; }
        public string Branch { get; set; }
        public string Agentid { get; set; }
        public string Pseudocity { get; set; }
        public string Acct { get; set; }
        public string Invoice { get; set; }
        public DateTime? Invdate { get; set; }
        public DateTime? Bookdate { get; set; }
        public string Domintl { get; set; }
        public string Ticket { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Break1 { get; set; }
        public string Break2 { get; set; }
        public string Break3 { get; set; }
        public string Break4 { get; set; }
        public decimal Airchg { get; set; }
        public string Credcard { get; set; }
        public string Cardnum { get; set; }
        public DateTime? Arrdate { get; set; }
        public decimal Mktfare { get; set; }
        public string Valcarr { get; set; }
        public DateTime? Depdate { get; set; }
        public decimal Stndchg { get; set; }
        public decimal Offrdchg { get; set; }
        public string Reascode { get; set; }
        public string Savingcode { get; set; }
        public string Bktool { get; set; }
        public string Bkagent { get; set; }
        public string Tkagent { get; set; }
        public string Tickettype { get; set; }
        public decimal Faretax { get; set; }
        public decimal Basefare { get; set; }
        public string Corpacct { get; set; }
        public string Sourceabbr { get; set; }
        public decimal Tax4 { get; set; }
        public decimal Tax3 { get; set; }
        public decimal Tax2 { get; set; }
        public decimal Tax1 { get; set; }
        public string Origticket { get; set; }
        public bool Exchange { get; set; }
        public string Moneytype { get; set; }
        public string Iatanbr { get; set; }
        public string Chgindcatr { get; set; }
        public string Trantype { get; set; }
        public int Plusmin { get; set; }
        public decimal Svcfee { get; set; }
        public decimal Acommisn { get; set; }
        public string Tourcode { get; set; }
        public string Cancelcode { get; set; }
        public string Phone { get; set; }
        public DateTime? Lastupdate { get; set; }
        public string Gds { get; set; }
        public string Changedby { get; set; }
        public DateTime? Changstamp { get; set; }
        public DateTime? Parsestamp { get; set; }
        public string AgContact { get; set; }
        public string Emailaddr { get; set; }

        //fieldList += @", trantype, convert(int,plusmin) as plusmin, svcfee, acommisn, tourcode, ' ' as cancelcode, ' ' as phone, " +
        //                 "' ' as gds, ' ' as changedby, invdate as changstamp, invdate as parsestamp, ' ' as agcontact, ' ' as emailaddr, invdate as lastupdate ";

        //computed columns
        public int PnrPaxCnt { get; set; }
        public string RecLoc6 { get; set; }
        public string PassNbr { get; set; }
        public int CommonKey { get; set; }
    }


}
