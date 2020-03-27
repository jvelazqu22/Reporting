using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Helper;
using Domain.Orm.Classes;
using Domain.Orm.iBankMastersQueries.ReportServer;
using iBank.Repository.SQL.Interfaces;

namespace iBank.ReportServer.Helpers
{
    public class PendingReportsRetriever
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IList<PendingReportInformation> GetPendingReports(IMasterDataStore masterStore, IList<int> demoUsers,
            int serverNumber, bool isDevMode, ReportServerFunction function)
        {
            var pendingReports = PullPendingReports(masterStore.MastersQueryDb, demoUsers, serverNumber).ToList();
            
            return GetServerTypeFilteredReports(masterStore, pendingReports, function, isDevMode);
        }

        private IList<PendingReportInformation> PullPendingReports(IMastersQueryable queryDb, IList<int> demoUsers, int serverNumber)
        {
            var pendingQuery = new GetPendingReportsQuery(queryDb, demoUsers, serverNumber);
            var list = pendingQuery.ExecuteQuery();

            return list;
        }

        private List<PendingReportInformation> GetServerTypeFilteredReports(IMasterDataStore masterStore, List<PendingReportInformation> pendingReports,
            ReportServerFunction function, bool isDevMode)
        {
            var filter = new PendingReportsFilter(masterStore);
            var list =  filter.GetFilteredReports(pendingReports, function, isDevMode).ToList();

            return list;
        }
    }
}
