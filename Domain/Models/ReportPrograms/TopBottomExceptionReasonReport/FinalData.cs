namespace Domain.Models.ReportPrograms.TopBottomExceptionReasonReport
{
    public class FinalData
    {
        public string Category { get; set; } = string.Empty;
        public string ReasCode { get; set; } = string.Empty;
        public int NumOccurs { get; set; } = 0;
        public decimal LostAmt { get; set; } = 0;
    }
}
