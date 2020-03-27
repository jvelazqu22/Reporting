using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravAuthKpi
{
    public class RawData : IRecKey
    {
        public int RecKey { get; set; }
        public string AuthStatus { get; set; }
        public int NumRecs { get; set; }
    }
}
