using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Extensions;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.ReportServer
{
    public class GetPendingReportsQuery : IQuery<IList<PendingReportInformation>>
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IMastersQueryable _db;
        private int ServerNumber { get; }
        private IList<int> DemoUsers { get; }

        public GetPendingReportsQuery(IMastersQueryable db, IList<int> demoUsers, int serverNumber)
        {
            _db = db;
            _db.UpdateTimeCommandTimeOut(120);
            DemoUsers = demoUsers;
            ServerNumber = serverNumber;
        }

        public IList<PendingReportInformation> ExecuteQuery()
        {

//#if DEBUG
//            // Use the DebugExecuteQuery when running in debug mode. Otherwise, you will get an error due to a bug with EF
//            return DebugExecuteQuery();
//#endif
            var listToReturn = new List<PendingReportInformation>();
            using (var scope = TransactionScopeBuilder.BuildNoLockScope())
            {
                try
                {
                    using (_db)
                    {
                        listToReturn = (from pr in _db.PendingReports
                                join rh in _db.ReportHandoff
                                    on pr.ReportId equals rh.reportid
                                where pr.IsDotNet
                                    && !pr.IsRunning
                                select new PendingReportInformation
                                {
                                    ReportId = rh.reportid,
                                    Agency = !string.IsNullOrEmpty(rh.agency) ? rh.agency.ToUpper().Trim() : "",
                                    ColdFusionBox = rh.cfbox,
                                    UserNumber = rh.usernumber,
                                    ServerNumber = ServerNumber,
                                    IsDemoUser = DemoUsers.Contains(rh.usernumber),
                                    RowVersion = pr.RowVersion,
                                    IsDotNet = pr.IsDotNet,
                                    DateCreated = rh.datecreated
                                }).GroupBy(x => x.ReportId)
                                  .Select(x => x.FirstOrDefault())
                                  .ToList();
                    }
                }
                finally
                {
                    scope.Complete();
                }
            }

            try
            {
                // for some reason this list can end up with null objects
                listToReturn.RemoveAll(w => w == null);
            }
            catch (Exception e)
            {
                LOG.Error(e);
            }

            return listToReturn;
        }

        // This is to get around an bug with EF that prevents running the ExecuteQuery above in debug mode.
        private IList<PendingReportInformation> DebugExecuteQuery()
        {
            var listToReturn = new List<PendingReportInformation>();
            using (var scope = TransactionScopeBuilder.BuildNoLockScope())
            {
                try
                {
                    using (_db)
                    {
                        var pendingReports = _db.PendingReports.Where(w => w.IsDotNet && !w.IsRunning).ToList();
                        var repndingReportIds = pendingReports.Select(s => s.ReportId).ToList();
                        var reportHandOffReports = _db.ReportHandoff.Where(w => repndingReportIds.Contains(w.reportid)).ToList();

                        var pendingReportInfoList = new List<PendingReportInformation>();
                        foreach (var rh in reportHandOffReports)
                        {
                            var temp = new PendingReportInformation()
                            {
                                ReportId = rh.reportid,
                                Agency = !string.IsNullOrEmpty(rh.agency) ? rh.agency.ToUpper().Trim() : "",
                                ColdFusionBox = rh.cfbox,
                                UserNumber = rh.usernumber,
                                ServerNumber = ServerNumber,
                                IsDemoUser = DemoUsers.Contains(rh.usernumber),
                                DateCreated = rh.datecreated
                            };

                            var pendingReport = pendingReports.First(w => w.ReportId.Equals(temp.ReportId));
                            temp.RowVersion = pendingReport.RowVersion;
                            temp.IsDotNet = pendingReport.IsDotNet;

                            pendingReportInfoList.Add(temp);
                        }

                        listToReturn = pendingReportInfoList.GroupBy(x => x.ReportId)
                                  .Select(x => x.FirstOrDefault())
                                  .ToList();
                    }
                }
                finally
                {
                    scope.Complete();
                }
                return listToReturn;
            }
        }
    }
}
