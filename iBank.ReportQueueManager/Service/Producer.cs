using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using com.ciswired.libraries.CISLogger;
using Domain.Orm;
using Domain.Orm.iBankMastersQueries.ReportQueueManager;
using iBank.Entities.MasterEntities;
using iBank.ReportQueueManager.Helpers;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Logging;

namespace iBank.ReportQueueManager.Service
{
    public class Producer
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IMasterDataStore _store;

        public Producer(IMasterDataStore store)
        {
            _store = store;
        }

        public void LoadReportsToValidate(QueueParameters parms, MaintenanceModeState maintState)
        {
            while (!parms.IsMaintenanceModeRequested)
            {
                try
                {
                    var pendingReports = GetPendingReportIds();
                    foreach (var report in pendingReports)
                    {
                        if (!IsOnPendingReportsTable(report.ReportId))
                        {
                            LOG.Debug($"Validating reportid [{report.ReportId}]");
                            parms.ReportsToValidate.Add(report);
                        }

                        Thread.Sleep(500);
                    }

                    parms.IsMaintenanceModeRequested = maintState.IsMaintenanceModeRequested();

                    if (parms.IsMaintenanceModeRequested)
                    {
                        LOG.Info("Maintenance mode requested. No longer monitoring reports.");
                        parms.ReportsToValidate.CompleteAdding();
                    }

                    //wait before checking queue again
                    var random = new Random();
                    Thread.Sleep(random.Next(1000,2000));
                }
                catch (Exception ex)
                {
                    parms.ReportsToValidate.CompleteAdding();
                    ErrorLogger.LogException(null, "", ex, MethodBase.GetCurrentMethod(), ServerType.ReportQueueManager, LOG);
                    break;
                }
            }
        }

        private IEnumerable<PendingReports> GetPendingReportIds()
        {
            var query = new GetQueuedReportHandoffsQuery(_store.MastersQueryDb);
            var pendingReports = query.ExecuteQuery();

            return pendingReports.Select(x => new PendingReports
                                            {
                                                Agency = x.agency.Trim(),
                                                ReportId = x.reportid.Trim(),
                                                UserNumber = x.usernumber,
                                                ReportHandOffDateCreated = x.datecreated
                                            });
        }

        private bool IsOnPendingReportsTable(string reportId)
        {
            var query = new IsOnPendingReportsTableQuery(_store.MastersQueryDb, reportId);
            return query.ExecuteQuery();
        }
    }
}
