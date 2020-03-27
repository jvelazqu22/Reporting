namespace Domain.Models.ReportPrograms.AirFareSavingsReport
{
    public class SummaryDataInformation
    {
        public SummaryDataInformation()
        {
            Description = string.Empty;
            Count = 0;
            Amount = 0m;
        }

        public int Count { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }
    }
}
