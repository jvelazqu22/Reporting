using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravAuthKpi
{
    public class OneValue : IRecKey
    {
        public int RecKey { get; set; }
        public int Value { get; set; }
    }
}
