using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using com.ciswired.libraries.CISLogger;
using iBank.Entities.MasterEntities;
using iBank.ReportQueueManager.Helpers;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Logging;

namespace iBank.ReportQueueManager.Service
{
    public class Consumer
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IMasterDataStore _store;

        private readonly IConfigurationWrapper _configWrapper;

        public Consumer(IMasterDataStore store, IConfigurationWrapper configWrapper)
        {
            _store = store;
            _configWrapper = configWrapper;
        }

        public void LoadPendingQueue(QueueParameters parms)
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = _configWrapper.MaxThreads };
            Parallel.ForEach(parms.ReportsToValidate.GetConsumingPartitioner(), options, report =>
            {
                ProcessReport(report, parms);
            });
        }

        private void ProcessReport(PendingReports report, QueueParameters parms)
        {
            try
            {
                if (parms.IsMaintenanceModeRequested) return;

                var handoff = new PendingReportHandler(report);
                handoff.SetResponsibleApp(_store, _configWrapper);
                handoff.AddToPendingReportTable(DateTime.Now, _store);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogException(null, "", ex, MethodBase.GetCurrentMethod(), ServerType.ReportQueueManager,
                    LOG);
            }
        }

    }
}
