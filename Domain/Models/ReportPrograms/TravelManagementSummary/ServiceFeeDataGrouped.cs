
namespace Domain.Models.ReportPrograms.TravelManagementSummary
{
    public class ServiceFeeDataGrouped : IGroupedByMonth
    {
        public int MonthNum { get; set; }
        public decimal SvcFee { get; set; }
        public int SvcFeeCnt { get; set; }
    }
}
