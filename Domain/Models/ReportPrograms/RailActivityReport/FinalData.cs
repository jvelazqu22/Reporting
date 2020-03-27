using System;

namespace Domain.Models.ReportPrograms.RailActivityReport
{
    public class FinalData
    {
        public FinalData()
        {
            Acct = string.Empty;
            AcctDesc = string.Empty;
            AirChg = 0;
            Airline = string.Empty;
            BookDate = DateTime.MinValue;
            Break1 = string.Empty;
            Break2 = string.Empty;
            Break3 = string.Empty;
            CardNum = string.Empty;
            Connect = string.Empty;
            DepDate = DateTime.MinValue;
            DestDesc = string.Empty;
            Destinat = string.Empty;
            Exchange = false;
            FltNo = string.Empty;
            InvDate = DateTime.MinValue;
            Invoice = string.Empty;
            OffRdChg = 0;
            OrgDesc = string.Empty;
            Origin = string.Empty;
            OrigTicket = string.Empty;
            PassFrst = string.Empty;
            PassLast = string.Empty;
            PlusMin = 0;
            PassFrst = string.Empty;
            PseudoCity = string.Empty;
            RdepDate = DateTime.MinValue;
            RecKey = 0;
            RecLoc = string.Empty;
            SeqNo = 0;
            SfTranType = string.Empty;
            SortDate = DateTime.MinValue;
            SvcFee = 0;
            Ticket = string.Empty;
            TktDesig = string.Empty;
            TranType = string.Empty;
            Classcode = string.Empty;
        }
        public string Classcode { get; set; }
        public string Acct { get; set; }
        public string AcctDesc { get; set; }
        public decimal AirChg { get; set; }
        public string Airline { get; set; }
        public DateTime BookDate { get; set; }
        public string Break1 { get; set; }
        public string Break2 { get; set; }
        public string Break3 { get; set; }
        public string CardNum { get; set; }
        public string Connect { get; set; }
        public DateTime DepDate { get; set; }
        public string DestDesc { get; set; }
        public string Destinat { get; set; }
        public bool Exchange { get; set; }
        public string FltNo { get; set; }
        public DateTime InvDate { get; set; }
        public string Invoice { get; set; }
        public decimal OffRdChg { get; set; }
        public string OrgDesc { get; set; }
        public string Origin { get; set; }
        public string OrigTicket { get; set; }
        public string PassFrst { get; set; }
        public string PassLast { get; set; }
        public int PlusMin { get; set; }
        public string PseudoCity { get; set; }
        public DateTime RdepDate { get; set; }
        public int RecKey { get; set; }
        public string RecLoc { get; set; }
        public int SeqNo { get; set; }
        public string SfTranType { get; set; }
        public DateTime SortDate { get; set; }
        public decimal SvcFee { get; set; }
        public string Ticket { get; set; }
        public string TktDesig { get; set; }
        public string TranType { get; set; }
    }
}
