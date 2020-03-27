using System;

namespace Domain.Models.ReportPrograms.DeparturesReport
{
    public class FinalData
    {
        public FinalData()
        {
            Mode = string.Empty;
            Airline = string.Empty;
            AlineDesc = string.Empty;
            ArrTime = string.Empty;
            CrysPgBrk = string.Empty;
            DepTime = string.Empty;
            Destdesc = string.Empty;
            Destinat = string.Empty;
            FrstDesc = string.Empty;
            FrstOrigin = string.Empty;
            FltNo = string.Empty;
            FltSort = string.Empty;
            Invoice = string.Empty;
            OrgDesc = string.Empty;
            Origin = string.Empty;
            PassFrst = string.Empty;
            PassLast = string.Empty;
            PlusMin = 0;
            Pseudocity = string.Empty;
            RdepDate = DateTime.MinValue;
            RecKey = 0;
            Recloc = string.Empty;
            Seg_Cntr = 0;
            SortDepTim = string.Empty;
            TxtDepDate = string.Empty;
            SeqNo = 0;
        }
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public int SeqNo { get; set; } = 0;
        public string Airline { get; set; } = string.Empty;
        public string AlineDesc { get; set; } = string.Empty;
        public string ArrTime { get; set; } = string.Empty;
        public string CrysPgBrk { get; set; } = string.Empty;
        public string DepTime { get; set; } = string.Empty;
        public string Destdesc { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string FrstDesc { get; set; } = string.Empty;
        public string FrstOrigin { get; set; } = string.Empty;
        public string FltNo { get; set; } = string.Empty;
        public string FltSort { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        public string OrgDesc { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string PassFrst { get; set; } = string.Empty;
        public string PassLast { get; set; } = string.Empty;
        public int PlusMin { get; set; } = 0;
        public string Pseudocity { get; set; } = string.Empty;
        public DateTime RdepDate { get; set; } = DateTime.MaxValue;
        public int RecKey { get; set; } = 0;
        public string Recloc { get; set; } = string.Empty;
        public int Seg_Cntr { get; set; } = 0;
        public int SegNum { get; set; } = 0;
        public string SortDepTim { get; set; } = string.Empty;
        public string TxtDepDate { get; set; } = string.Empty;
    }
}
