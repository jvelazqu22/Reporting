
using Domain.Models.ReportPrograms.TravelManagementSummary;

namespace Domain.Models.ReportPrograms.TravelManagementSummary
{
    public class LegDataGrouped : IGroupedByMonth
    {
        public int MonthNum { get; set; }
        public int TotMiles { get; set; }
        public int DomMiles { get; set; }
        public int IntlMiles { get; set; }
        public decimal TotAirCo2 { get; set; }
        public decimal DomAirCo2 { get; set; }
        public decimal IntlAirCo2 { get; set; }
    }
}
