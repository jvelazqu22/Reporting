using CODE.Framework.Core.Utilities.Extensions;
using com.ciswired.libraries.CISLogger;
using Domain.Models.BroadcastServer;
using Domain.Orm.iBankAdministrationQueries;
using iBank.BroadcastServer.BroadcastBatch;
using iBank.Server.Utilities.Cleaners;
using iBank.Server.Utilities.Logging;
using System.Collections.Concurrent;
using System.Configuration;
using System.Reflection;
using System.Threading.Tasks;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared;

namespace iBank.BroadcastServer.Service
{
    public class BroadcastServer
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        Parameters p;
        private readonly LoadedListsParams _loadedParams;
        public BroadcastServer(IMasterDataStore masterDataStore, LoadedListsParams loadedParams)
        {
            p = new Parameters(new BroadcastQueueRecordRemover(), new BroadcastRecordUpdatesManager())
            {
                MasterDataStore = masterDataStore,
                DatabaseInfoQuery = null,
                ClientDataStore = null
            };
            LoadConfiguration();
            _loadedParams = loadedParams;
        }
        
        private void LoadConfiguration()
        {
            p.ServerConfiguration = new BroadcastServerInformation
            {
                ReportLogoDirectory = ConfigurationManager.AppSettings["LogoTempDirectory"],
                CrystalReportDirectory = ConfigurationManager.AppSettings["CrystalReportsDirectory"],
                ServerNumber = ConfigurationManager.AppSettings["ServerNumber"].TryIntParse(0),
                ReportOutputDirectory = ConfigurationManager.AppSettings["ReportOutputDirectory"],
                SenderEmailAddress = ConfigurationManager.AppSettings["SenderEmailAddress"],
                SenderName = ConfigurationManager.AppSettings["SenderName"],
                IbankBaseUrl = ConfigurationManager.AppSettings["IbankBaseUrl"]
            };

            var getServerFunctionQuery = new GetBroadcastServerFunctionByServerNumberQuery(new iBankAdministrationQueryable(), p.ServerConfiguration.ServerNumber);
            p.ServerConfiguration.ServerFunction = getServerFunctionQuery.ExecuteQuery();
            if (p.ServerConfiguration.ServerNumber == 0)
            {
                LOG.Error($"Missing server number configuration from BroadcastServer config file.");
                p.ServerConfiguration.ServerNumber = 20;
            }
            LOG.Info($"Server [{p.ServerConfiguration.ServerNumber}] acting as [{p.ServerConfiguration.ServerFunction}] type server");
        }

        public void Run()
        {
            var maintenanceModeState = new MaintenanceModeState(ServerType.BroadcastServer, p.ServerConfiguration.ServerNumber);
            p.IsMaintenanceModeRequested = maintenanceModeState.IsMaintenanceModeRequested();

            if (p.IsMaintenanceModeRequested)
            {
                LOG.Debug("Entering maintenance mode.");
                maintenanceModeState.EnterMaintenanceMode();
                return;
            }
            
            LOG.Debug("Entering loop to retrieve batches.");
            
            p.BatchesToExecute = new BlockingCollection<bcstque4>();
            Task.Run(() => new Producer().LoadBatchesToExecute(maintenanceModeState, p));

            new Consumer().ProcessBatches(p, _loadedParams);

            if (p.IsMaintenanceModeRequested)
            {
                LOG.Debug("Entering maintenance mode.");
                maintenanceModeState.EnterMaintenanceMode();
            }
        }
    }
}
