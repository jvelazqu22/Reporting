using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Reflection;
using System.Threading;
using com.ciswired.libraries.CISLogger;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Exceptions;
using Domain.Helper;
using Domain.Orm.Classes;
using Domain.Orm.iBankMastersQueries.Other;
using Domain.Services;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Logging;
using iBank.Server.Utilities.ReportCriteriaHandlers;
using iBank.Server.Utilities.ReportGlobalsSetters;
using iBank.Services.Implementation.Classes.Interfaces;
using iBank.Services.Implementation.Shared.Client;
using iBank.Services.Implementation.Utilities.ReportSetup;

namespace iBank.Services.Implementation.ReportPrograms
{
    public class ReportSwitch
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly IReportServerLogger RPT_LOG = new ReportServerLogger(new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType));
        public ManualResetEvent DoneEvent { get; set; }

        public bool IsOfflineServer { get; set; }
        public bool DevMode { get; set; }
        public int FinalRecordsCount { get; set; }
        public Dictionary<int,string> WhereCriteria { get; set; }

        public int ServerNumber { get; set; }
        public IReportRunner ReportRunner { get; set; }

        public IMasterDataStore MasterDataStore { get; set; }

        public ReportSwitch()
        {
            MasterDataStore = new MasterDataStore();
            ServerNumber = ConfigurationManager.AppSettings["ServerNumber"].TryIntParse(-1);
        }

        public ReportSwitch(IMasterDataStore masterDataStore, int serverNumber)
        {
            MasterDataStore = masterDataStore;
            ServerNumber = serverNumber;
        }
        
        public void RunReport(PendingReportInformation report, LoadedListsParams loadedParams)
        {
            try
            {
                var statusHandler = new ReportStatusHandler();
                var reportStatusRecord = statusHandler.RetrieveReportStatusRecord(report, MasterDataStore);

                if (!DevMode && !statusHandler.IsRecordAvailableToRun(reportStatusRecord))
                {
                    RPT_LOG.DebugLogWithReportInfo(report, "Report no longer considered PENDING.");
                    DoneEvent.Set();
                    return;
                }

                statusHandler.SetStatusToInProcess(MasterDataStore, ServerNumber, reportStatusRecord);

                var buildWhere = new BuildWhere(new ClientFunctions());
                buildWhere.ReportGlobals = GetGlobals(report, MasterDataStore, new CacheService());
                
                var logging = new ReportingLog(ReportLogLogger.ReportMode.REGULAR);
                try
                {
                    logging.StartLogging(buildWhere.ReportGlobals, new ApplicationInformation(Assembly.GetExecutingAssembly()));
                }
                catch (Exception ex)
                {
                    var msg = RPT_LOG.FormatWithReportInfo(report);
                    ErrorLogger.LogException(report.UserNumber, report.Agency, ex, msg, MethodBase.GetCurrentMethod(), ServerType.BroadcastServer, LOG);
                }
                
                try
                {
                    var reportRunnerFactory = new ReportRunnerFactory(buildWhere.ReportGlobals.ProcessKey, buildWhere, loadedParams);
                    ReportRunner = reportRunnerFactory.Build();
                }
                catch (ReportNotSupportedException ex)
                {
                    report.ErrorMessage = new ReportMessages().RptMsg_ReportNotSupported;
                    ErrorLogger.LogException(report.UserNumber, report.Agency, ex, MethodBase.GetCurrentMethod(), ServerType.OnlineReportServer, LOG);
                    DoneEvent.Set();
                    return;
                }

                RPT_LOG.InfoLogWithReportInfo(report, $"Running report with process key [{buildWhere.ReportGlobals.ProcessKey}]");
                report.ReportLogKey = buildWhere.ReportGlobals.ReportLogKey;

                if (ReportRunner.RunReport())
                {
                    FinalRecordsCount = ReportRunner.GetFinalRecordsCount();
                    ReportHelper.EndReport(report, buildWhere.ReportGlobals);
                }
                else
                {
                    ReportHelper.PostError(report);
                }
            }
            catch (DbEntityValidationException ex)
            {
                var error = ex.GetValidationErrors();
                ErrorLogger.LogException(report.UserNumber, report.Agency, ex, error, MethodBase.GetCurrentMethod(), ServerType.OnlineReportServer, LOG);
                
                report.ErrorMessage = new ReportMessages().GenericErrorMessage;
                ReportHelper.PostError(report);
            }
            catch (Exception e)
            {
                ErrorLogger.LogException(report.UserNumber, report.Agency, e, MethodBase.GetCurrentMethod(), ServerType.OnlineReportServer, LOG);
                
                report.ErrorMessage = new ReportMessages().GenericErrorMessage;
                ReportHelper.PostError(report);
            }
            finally
            {
                DoneEvent.Set();
            }
        }

        private ReportGlobals GetGlobals(PendingReportInformation report, IMasterDataStore store, ICacheService cache)
        {
            var temp = ConfigurationManager.AppSettings["ServerNumber"];
            if (!int.TryParse(temp, out var svrNumber)) throw new Exception("Invalid Server Number!");

            var reportCritQuery = new GetReportInputCriteriaByReportIdQuery(store.MastersQueryDb, report.ReportId);
            var collistQuery = new GetActiveColumnsQuery(store.MastersQueryDb);
            WhereCriteria = WhereCriteriaLookup.GetWhereCriteriaLookup(cache, store);

            var parms = new ReportGlobalsCreatorParams.Builder()
                                                      .WithStandardCritRetriever(new StandardReportCritieraRetriever())
                                                      .WithAdvancedParamRetriever(new AdvancedParameterRetriever())
                                                      .WithUdidRetriever(new MultiUdidParameterRetriever())
                                                      .WithPendingReportInformation(report)
                                                      .WithCrystalDirectory(ConfigurationManager.AppSettings["CrystalReportsDirectory"])
                                                      .WithIbankVersion(ConfigurationManager.AppSettings["iBankVersion"])
                                                      .WithOfflineServerDesignation(IsOfflineServer)
                                                      .WithWhereCriteriaLookup(WhereCriteria)
                                                      .WithReportCriteriaQuery(reportCritQuery)
                                                      .WithActiveColumnsQuery(collistQuery)
                                                      .Build();
            
            return ReportGlobalsCreator.CreateFromOnlineReport(parms, svrNumber);
        }
        
    }
}
