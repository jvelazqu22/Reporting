using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravAuthKpi
{
    public class KpiTravelersRawData : IRecKey
    {
        public KpiTravelersRawData()
        {
            PassFrst = string.Empty;
            PassLast = string.Empty;
            AuthStatus = string.Empty;
            RecKey = 0;
        }
        public string PassLast { get; set; }
        public string PassFrst { get; set; }
        public string AuthStatus { get; set; }
        public int NumRecs { get; set; }
        public int RecKey { get; set; }
    }
}
