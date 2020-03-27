using Domain.Helper;

namespace Domain.Models.ReportPrograms.AgentSummary
{
    [Exportable]
    public class FinalData
    {
        public string Agentid { get; set; } = string.Empty;
        public int Transacts { get; set; } = 0;
        public decimal Tickets { get; set; } = 0m;
        public decimal Refunds { get; set; } = 0m;
        public decimal Net_trips { get; set; } = 0m;
        public decimal Commission { get; set; } = 0m;
        public decimal Invoiceamt { get; set; } = 0m;
        public decimal Creditamt { get; set; } = 0m;
        public decimal Netvolume { get; set; } = 0m;

    }
}
