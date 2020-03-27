using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravAuthKpi
{
    public class ReasonCodeRawData :IRecKey
    {
        public ReasonCodeRawData()
        {
            RecKey = 0;
            Acct = string.Empty;
            OutPolCods = string.Empty;
            NumTrips = 0;
            Cost = 0;
        }

        public int RecKey { get; set; }
        public string Acct { get; set; }
        public string OutPolCods { get; set; }
        public int NumTrips { get; set; }
        public decimal Cost { get; set; }
    }
}
