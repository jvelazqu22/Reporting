using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using com.ciswired.libraries.CISLogger;
using iBank.Entities.MasterEntities;
using iBank.ReportQueueManager.Helpers;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Logging;

namespace iBank.ReportQueueManager.Service
{
    public class QueueManager
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IMasterDataStore MasterStore { get; set; }
        private readonly IConfigurationWrapper _wrapper;

        public QueueManager(IMasterDataStore store, IConfigurationWrapper wrapper)
        {
            LOG.Debug($"Start {MethodBase.GetCurrentMethod().Name} ");

            MasterStore = store;
            _wrapper = wrapper;
        }

        public void Run(MaintenanceModeState state)
        {
            LOG.Debug($"Start {MethodBase.GetCurrentMethod().Name} ");

            var parms = new QueueParameters {IsMaintenanceModeRequested = state.IsMaintenanceModeRequested()};

            if (parms.IsMaintenanceModeRequested)
            {
                LOG.Info("Entering maintenace mode");
                state.EnterMaintenanceMode();
                return;
            }

            LOG.Debug("Entering loop to retrieve pending reports.");
            parms.ReportsToValidate = new BlockingCollection<PendingReports>();
            
            Task.Run(() =>
             {
                 // read reports from the reporthandoff table and add them into the parms.ReportsToValidate list
                 var producer = new Producer(MasterStore);
                 producer.LoadReportsToValidate(parms, state);
             });

            // reads records fromt he parms.ReportsToValidate list and determines if pending record(s) is/are .net or foxpro, and add record to pending_reports table
            var consumer = new Consumer(MasterStore, _wrapper);
            consumer.LoadPendingQueue(parms);
        }

        public void RunCleaner()
        {
            LOG.Debug($"Start {MethodBase.GetCurrentMethod().Name} ");

            //var temp = ConfigurationManager.AppSettings["TimeInHoursToKeepPendingReports"].TryIntParse(1);
            //var threshold = DateTime.Now.AddHours(-temp);
            var threshold = DateTime.Now.AddHours(-1);
            new QueueCleaner().CleanQueue(threshold, new MasterDataStore());
        }
    }
}
