
namespace Domain.Models.ReportPrograms.WtsFareSavings
{
    public class SummaryData
    {
        public string ReasCode { get; set; } = string.Empty;
        public string ReasDesc { get; set; } = string.Empty;
        public decimal ReasCount { get; set; } = 0m;
        public decimal Savings { get; set; } = 0m;
        public decimal LostAmt { get; set; } = 0m;

    }
}
