
using Domain.Models.ReportPrograms.TravelManagementSummary;

namespace Domain.Models.ReportPrograms.TravelManagementSummary
{
    public class RailDataGrouped : IGroupedByMonth
    {
        public int MonthNum { get; set; }
        public decimal FullFare { get; set; }
        public decimal GrossRail { get; set; }
        public decimal NetRail { get; set; }
        public int Invoices { get; set; }
        public decimal LostAmt { get; set; }
        public decimal LowFare { get; set; }
        public decimal NegoSvngs { get; set; }
        public int NetTrans { get; set; }
        public decimal RefundAmt { get; set; }
        public decimal Savings { get; set; }
        public int Refunds { get; set; }
    }
}
