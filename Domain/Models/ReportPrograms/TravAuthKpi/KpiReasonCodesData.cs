
namespace Domain.Models.ReportPrograms.TravAuthKpi
{
    public class KpiReasonCodesData
    {
        public KpiReasonCodesData()
        {
            Reasdesc = string.Empty;
            Numtrips = 0;
            Totalcost = 0m;
        }
        public string Reasdesc { get; set; }
        public int Numtrips { get; set; }
        public decimal Totalcost { get; set; }

    }
}
