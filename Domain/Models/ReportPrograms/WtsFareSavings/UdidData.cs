using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.WtsFareSavings
{
    public class UdidData : IRecKey
    {
        public int RecKey { get; set; } = 0;
        public string UdidText { get; set; } = string.Empty;
    }
}
