using System;

namespace Domain.Models.ReportPrograms.ArrivalReport
{
    public class FinalData
    {
        public int RecKey { get; set; } = 0;
        public string Destinat { get; set; } = string.Empty;
        public string Destdesc { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        public string Recloc { get; set; } = string.Empty;
        public int PlusMin { get; set; } = 0;
        public DateTime RArrDate { get; set; } = DateTime.MinValue;
        public string Acct { get; set; } = string.Empty;
        public string AcctDesc { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string PassLast { get; set; } = string.Empty;
        public string PassFrst { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string AlineDesc { get; set; } = string.Empty;
        public string Pseudocity { get; set; } = string.Empty;
        public string FltNo { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string OrgDesc { get; set; } = string.Empty;
        public string ArrTime { get; set; } = string.Empty;
        public string FltSort { get; set; } = string.Empty;
        public string SortArrTim { get; set; } = string.Empty;
        public int Seg_Cntr { get; set; } = 0;
        public string TxtArrDate { get; set; } = string.Empty;
        public string CrysPgBrk { get; set; } = string.Empty;
        public string DepTime { get; set; } = string.Empty;
        public string FinalDest { get; set; } = string.Empty;
        public string FinalDesc { get; set; } = string.Empty;
        public int SeqNo { get; set; } = 0;
        public string Mode { get; set; } = string.Empty;
        public int SegNum { get; set; } = 0;
    }
}
