using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ExecutiveSummaryByHomeCountryReport
{
    public class LegRawData : IRecKey
    {
        public int RecKey { get; set; } = 0;
        public string Airline { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
    }
}
