using System;

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.DocumentDeliveryLog
{
    public class RawData : IRecKey
    {
        public RawData()
        {
            RecKey = 0;
            Acct = string.Empty;
            Depdate = DateTime.Now;
            Recloc = string.Empty;
            Passlast = string.Empty;
            Passfrst = string.Empty;
            Bookedgmt = DateTime.MinValue;
            TravAuthNo = 0;
            Rtvlcode = string.Empty;
            OutPolCods = string.Empty;
            Sgroupnbr = 0;
            AuthStatus = string.Empty;
            Statustime = DateTime.MinValue;
            Gds = string.Empty;
            Authlognbr = 0;
            Statusnbr = 0;
            DocStatTim = DateTime.MinValue;
            DocSuccess = true;
            DocType = string.Empty;
            DocRecips = string.Empty;
            DocSubject = string.Empty;
            DocText = string.Empty;
            DocHtml = string.Empty;
            DlvRespons = string.Empty;
        }
        public int RecKey { get; set; }
        public string Acct { get; set; }
        public DateTime? Depdate { get; set; }
        public string Recloc { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public DateTime? Bookedgmt { get; set; }
        public int TravAuthNo { get; set; }
        public string Rtvlcode { get; set; }
        public string OutPolCods { get; set; }
        public int Sgroupnbr { get; set; }
        public string AuthStatus { get; set; }
        public DateTime? Statustime { get; set; }
        public string Gds { get; set; }
        public int Authlognbr { get; set; }
        public int Statusnbr { get; set; }
        public DateTime? DocStatTim { get; set; }
        public bool DocSuccess { get; set; }
        public string DocType { get; set; }
        public string DocRecips { get; set; }
        public string DocSubject { get; set; }
        public string DocText { get; set; }
        public string DocHtml { get; set; }
        public string DlvRespons { get; set; }
    }
}
