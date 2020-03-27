using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using com.ciswired.libraries.CISLogger;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Models.OnlineReportServer;
using Domain.Orm.iBankAdministrationQueries;
using Domain.Services;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Logging;
using iBank.Services.Implementation.Shared;

namespace iBank.ReportServer.Service
{
    public class ReportServer
    {
        private static readonly com.ciswired.libraries.CISLogger.ILogger LOG = new LogManagerWrapper().GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Parameters _p;
        private LoadedListsParams _loadedParams;

        public ReportServer(IMasterDataStore masterDataStore, bool devMode, LoadedListsParams loadedParams)
        {
            _p = new Parameters(masterDataStore);
            _p.ServerNumber = ConfigurationManager.AppSettings["ServerNumber"].TryIntParse(0);
            var demo = ConfigurationManager.AppSettings["DemoVersion"];
            _p.IsDemoVersion = demo != null && demo.EqualsIgnoreCase("true");
            _p.DemoUsers = GetDemoUsers(_p.IsDemoVersion);
            _p.IsDevMode = devMode;
            _p.Cache = new CacheService();

            _p.ServerFunction = new GetReportServerFunctionByServerNumberQuery(new iBankAdministrationQueryable(), _p.ServerNumber).ExecuteQuery();
            _loadedParams = loadedParams;
            LOG.Info($"Server [{_p.ServerNumber}] acting as [{_p.ServerFunction}]");
        }

        public void Run()
        {
            LOG.Debug($"Server [{_p.ServerNumber}] attempting to run.");
            var maintenanceModeState = new MaintenanceModeState(ServerType.OnlineReportServer, _p.ServerNumber);
            _p.IsMaintenanceModeRequested = maintenanceModeState.IsMaintenanceModeRequested();
            if (_p.IsMaintenanceModeRequested)
            {
                LOG.Info("Entering maintenance mode");
                maintenanceModeState.EnterMaintenanceMode();
                return;
            }

            // Get reports from ReportHandOff table, determine if report can be process, filter reports based on devmode, primary, or stage servers, 
            // read records from the pending_reports table and add them to the _p.ReportsToExecute list if not already there
            //#if DEBUG
            //            new Producer().LoadReportsDebugVersion(maintenanceModeState, _p);
            //#else
            Task.Run(() => new Producer().LoadReportsToRun(maintenanceModeState, _p));
//#endif

            // read reports from the p.ReportsToExecute list, and race to update status in IsRunning = true. If race is lost exit. Otherwise process report.
            new Consumer().ProcessReports(_p, _loadedParams);

            if (_p.IsMaintenanceModeRequested)
            {
                LOG.Info("Maintenance mode requested.");
                maintenanceModeState.EnterMaintenanceMode();
            }
        }

        private IList<int> GetDemoUsers(bool isDemoVersion)
        {
            var demoUserList = new List<int>();
            if (isDemoVersion)
            {
                var demoUsers = ConfigurationManager.AppSettings["DemoUsers"];
                if (demoUsers != null)
                {
                    var temp = demoUsers.Split(',');
                    foreach (var item in temp)
                    {
                        int userNumber;
                        if (int.TryParse(item, out userNumber))
                        {
                            demoUserList.Add(userNumber);
                        }
                    }
                }
            }
            return demoUserList;
        }
    }
}
