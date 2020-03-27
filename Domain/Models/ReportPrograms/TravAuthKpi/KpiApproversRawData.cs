using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravAuthKpi
{
    public class KpiApproversRawData : IRecKey
    {
        public KpiApproversRawData()
        {
            AuthStatus = string.Empty;
            AuthrzrNbr = 0;
            Auth1Email = string.Empty;
            NumRecs = 0;
            RecKey = 0;
        }
        public int AuthrzrNbr { get; set; }
        public string Auth1Email { get; set; }
        public string AuthStatus { get; set; }
        public int NumRecs { get; set; }
        public int RecKey { get; set; }
    }
}
