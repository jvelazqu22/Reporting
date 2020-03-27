using System.Collections.Generic;
using Domain.Helper;
using iBank.Entities.MasterEntities;
using iBankDomain.RepositoryInterfaces;

namespace iBank.UnitTesting.iBank.ReportServer.Helpers
{
    public class PendingReportRetrieverContext
    {
        public IList<reporthandoff> Reports { get; set; }

        public IList<report_server_stage> ReportServerStage { get; set; }
        public IList<PendingReports> PendingReports { get; set; }

        public IList<mstragcy> MstrAgcy { get; set; }

        public IList<iBankDatabases> iBankDatabases { get; set; }
        public ICommandDb MastersCommandDb { get; set; }

        public bool IsDevMode { get; set; }

        public ReportServerFunction Function { get; set; }

        public PendingReportRetrieverContext()
        {
            Reports = new List<reporthandoff>();
            ReportServerStage = new List<report_server_stage>();
            MstrAgcy = new List<mstragcy>();
            PendingReports = new List<PendingReports>();
            iBankDatabases = new List<iBankDatabases>();
            IsDevMode = false;
        }
    }
}
