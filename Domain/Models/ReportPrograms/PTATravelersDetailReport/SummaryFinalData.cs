
namespace Domain.Models.ReportPrograms.PTATravelersDetailReport
{
    public class SummaryFinalData
    {
        public string AuthStatus { get; set; } = string.Empty;
        public string Statusdesc { get; set; } = string.Empty;
        public decimal Numtrips { get; set; } = 0m;
        public decimal Airchg { get; set; } = 0m;
        public decimal Carcost { get; set; } = 0m;
        public decimal Hotelcost { get; set; } = 0m;
        public decimal Tottripchg { get; set; } = 0m;
    }
}
