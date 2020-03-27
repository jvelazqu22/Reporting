
namespace Domain.Models.ReportPrograms.TravAuthKpi
{
    public class WhereClauses
    {

        public WhereClauses()
        {
           WhereCyMth = string.Empty;
           WhereCyYtd = string.Empty;
           WherePyMth = string.Empty;
           WherePyYtd = string.Empty;
        }

        public string WhereCyMth { get; set; }
        public string WhereCyYtd { get; set; }
        public string WherePyMth { get; set; }
        public string WherePyYtd { get; set; }
    }
}
