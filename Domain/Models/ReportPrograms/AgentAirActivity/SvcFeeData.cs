using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.AgentAirActivity
{
    public class SvcFeeData :IRecKey
    {
        public int RecKey { get; set; } = 0;
        public decimal SvcFee { get; set; } = 0;
    }
}
