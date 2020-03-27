
namespace Domain.Models.ReportPrograms.TravelManagementSummary
{
    public class AirDataGrouped : IGroupedByMonth
    {
        public decimal DomAir { get; set; }
        public int DomTrans { get; set; }
        public int Exchanges { get; set; }
        public decimal ExchngAmt { get; set; }
        public decimal FullFare { get; set; }
        public decimal GrossAir { get; set; }
        public decimal IntlAir { get; set; }
        public int IntlTrans { get; set; }
        public int Invoices { get; set; }
        public decimal LostAmt { get; set; }
        public decimal LowFare { get; set; }
        public int MonthNum { get; set; }
        public decimal NegoSvngs { get; set; }
        public decimal NetAir { get; set; }
        public int NetTrans { get; set; }
        public decimal OnlineAmt { get; set; }
        public int OnlineTkts { get; set; }
        public decimal RefundAmt { get; set; }
        public int Refunds { get; set; }
        public decimal Savings { get; set; }
    }
}
