using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.XmlExtractReport
{


    public class SvcFeeRawData : IRecKey
    {
        public SvcFeeRawData()
        {
            RecKey = 0;
            Recloc = string.Empty;
            Invoice = string.Empty;
            Acct = string.Empty;
            Passlast = string.Empty;
            Passfrst = string.Empty;
            SvcFee = 0;
            SvcDesc = string.Empty;
            Agency = string.Empty;
            Mco = string.Empty;
            SfCardnum = string.Empty;
            Trandate = DateTime.MinValue;
            SfTranType = string.Empty;
        }
        public int RecKey { get; set; }
        public string Recloc { get; set; }
        public string Invoice { get; set; }
        public string Acct { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public decimal SvcFee { get; set; }
        public string SvcDesc { get; set; }
        public string Agency { get; set; }
        public string Mco { get; set; }
        public string SfCardnum { get; set; }
        public DateTime? Trandate { get; set; }
        public string SfTranType { get; set; }
    }

}
