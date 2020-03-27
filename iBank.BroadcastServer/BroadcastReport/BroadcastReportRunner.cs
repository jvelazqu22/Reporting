using com.ciswired.libraries.CISLogger;
using Domain.Helper;
using Domain.Models.BroadcastServer;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Logging;
using iBank.Services.Implementation.Shared;

using System;
using System.Data.Entity.Core;
using System.Reflection;

using Domain.Exceptions;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.BroadcastServer.Helper;
using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.ReportGlobalsSetters;

using iBankDomain.Exceptions;
using Domain;

namespace iBank.BroadcastServer.BroadcastReport
{
    public class BroadcastReportRunner
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly IBroadcastLogger BCST_LOG = new BroadcastLogger(new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType));

        public ReportGlobals Globals { get; set; }
        
        private IMasterDataStore MasterDataStore { get; set; }
        private IClientDataStore ClientDataStore { get; set; }
        
        public BroadcastReportRunner(IMasterDataStore masterDataStore, IClientDataStore clientDataStore)
        {
            MasterDataStore = masterDataStore;
            ClientDataStore = clientDataStore;
            Globals = new ReportGlobals();
        }

        internal ReportRunResults RunReport(bcstque4 batch, BroadcastReportInformation report, BroadcastServerInformation config, ibuser user, LoadedListsParams loadedParams)
        {
            var results = new ReportRunResults();
            var rptLogger = new ReportLogLogger();

            Globals.ProcessKey = report.ProcessKey;
            Globals.Agency = batch.agency.Trim();
            Globals.BatchNumber = batch.batchnum ?? 0;
            Globals.ServerNumber = config.ServerNumber;

            //if rptusernum is a different user number that means the report is set to use someone else's user settings
            if (batch.rptusernum.ZeroIfNull() != batch.UserNumber && batch.rptusernum.ZeroIfNull() != 0)
            {
                var userQuery = new GetUserByUserNumberQuery(ClientDataStore.ClientQueryDb, batch.rptusernum.ZeroIfNull());
                var rptuser = userQuery.ExecuteQuery();

                if (rptuser != null)
                {
                    Globals.SetParmValue(WhereCriteria.COUNTRY, rptuser.country);
                    Globals.UserNumber = batch.rptusernum.ZeroIfNull();
                }
                else
                {
                    LOG.Warn($"Broadcast is set to run as another user. Corresponding user record for user number [{batch.rptusernum}] does not exist.");
                }
            }
            else
            {
                Globals.UserNumber = batch.UserNumber.ZeroIfNull();
            }
            
            Globals.IsOfflineServer = config.IsOfflineServer;

            var globalsSetter = new BroadcastReportGlobalsSetter(Globals, MasterDataStore.MastersQueryDb, ClientDataStore.ClientQueryDb);
            var savedReport = report.SavedReportNumber > 0;
            //get the criteria
            if (savedReport)
            {
                globalsSetter.SetReportGlobalsForSavedReport(batch, report);
            }
            else
            {
                globalsSetter.SetReportGlobalsForStandardReport(batch, report);
            }

            //TODO: clean this up - does not need to be repeated - this piece should stay
            //if rptusernum is a different user number that means the report is set to use someone else's user settings
            if (batch.rptusernum.ZeroIfNull() != batch.UserNumber && batch.rptusernum.ZeroIfNull() != 0)
            {
                var userQuery = new GetUserByUserNumberQuery(ClientDataStore.ClientQueryDb, batch.rptusernum.ZeroIfNull());
                var rptuser = userQuery.ExecuteQuery();
                if (rptuser != null)
                {
                    Globals.SetParmValue(WhereCriteria.COUNTRY, rptuser.country);
                    Globals.UserNumber = batch.rptusernum.ZeroIfNull();
                }
                else
                {
                    LOG.Warn($"Broadcast is set to run as another user. Corresponding user record for user number [{batch.rptusernum}] does not exist.");
                }
            }

            if (!CheckUserReportRestrictions(Globals.UserNumber, report.ProcessKey))
            {
                var msg = string.Format("User [{0}] not authorized for report key [{1}]", Globals.UserNumber, report.ProcessKey);
                ErrorLogger.LogWarning(Globals.UserNumber, batch.agency, msg, MethodBase.GetCurrentMethod(), ServerType.BroadcastServer, LOG);
                
                return new ReportRunResults { ReturnCode = 2, ErrorMessage = "Not Authorized", ReportRunSuccess = true };
            }
            
            //Globals that apply to Online and Broadcast reports
            new GlobalValuesSetter().SetGlobals(Globals);
            globalsSetter.SetAccountValues(batch);
            globalsSetter.SetSpecialCasesForSpecificReports(batch, report);
            
            globalsSetter.SetMoneyType(Globals.UserNumber, batch);
            globalsSetter.SetOutputValues();
            Globals.EProfileNumber = batch.eProfileNo;
            
            results.ReportPeriodStartDate = globalsSetter.ReportPeriodStartDate;
            results.ReportPeriodEndDate = globalsSetter.ReportPeriodEndDate;

            //Update Globals ReportParameters BegDate and EndDate to sync with ReportPeriodStartDate and ReportPeriodEndDate respectively
            //so the report criteria will be saved correctly into ibRptLogCrit table
            Globals.SetParmValue(WhereCriteria.BEGDATE, $"DT:{globalsSetter.ReportPeriodStartDate.Year},{globalsSetter.ReportPeriodStartDate.Month},{globalsSetter.ReportPeriodStartDate.Day}");
            Globals.SetParmValue(WhereCriteria.ENDDATE, $"DT:{globalsSetter.ReportPeriodEndDate.Year},{globalsSetter.ReportPeriodEndDate.Month},{globalsSetter.ReportPeriodEndDate.Day}");
            
            Globals.CrystalDirectory = config.CrystalReportDirectory;

            SetGlobalFixedDateCurrencyConversion();
            
            var handoff = new ServiceHandoff();
            var isOfflineBatch = BroadcastBatchTypeConditionals.IsBatchRunOffline(batch.batchname);
            var logger = new ReportingLog(isOfflineBatch ? ReportLogLogger.ReportMode.OFFLINE : ReportLogLogger.ReportMode.BROADCAST);

            try
            {
                logger.StartLogging(Globals, new ApplicationInformation(Assembly.GetExecutingAssembly()));
            }
            catch (Exception ex)
            {
                var msg = BCST_LOG.FormatBroadcastInfo(batch);
                var errorNumber = ErrorLogger.LogException(Globals.UserNumber, Globals.Agency, ex, msg, MethodBase.GetCurrentMethod(), ServerType.BroadcastServer, LOG);
                if (batch.send_error_email)
                {
                    new ErrorHandler().SendErrorEmail(LOG, batch, errorNumber, MasterDataStore, config, report);
                }
            }

            try
            {
                if (report.IsDotNetEnabled)
                {
                    var msg = $"Running report process key [{Globals.ProcessKey}] as .NET.".FormatMessageWithReportLogKey(Globals.ReportLogKey);
                    BCST_LOG.InfoLogWithBatchInfo(batch, msg);
                    results.ReportRunSuccess = handoff.RunNetReport(Globals, loadedParams);
                    results.FinalRecordsCount = handoff.FinalRecordsCount;
                    results.ReportFileName = Globals.SavedReportName;
                }
                else
                {
                    var msg = $"Handing report process key [{Globals.ProcessKey}] to FoxPro server.".FormatMessageWithReportLogKey(Globals.ReportLogKey);
                    BCST_LOG.InfoLogWithBatchInfo(batch, msg);
                    var globalValues = Globals;

                    try
                    {
                        results.ReportRunSuccess = handoff.RunFoxProReport(savedReport, ref globalValues, results.ReportPeriodStartDate,
                            results.ReportPeriodEndDate, config.ServerFunction);
                        results.ReportFileName = handoff.FoxProReportId;
                    }
                    catch (EntityCommandExecutionException e)
                    {
                        //this will throw either a RecoverableSqlException or the original exception
                        EFExceptionHandling.HandleEntityCommandExecutionException(e);
                    }

                    Globals = globalValues;
                }

                results.ErrorMessage = Globals.ReportInformation.ErrorMessage;
                results.ReportHref = Globals.ReportInformation.Href;
                results.ReportHasNoData = results.ErrorMessage.Equals(Globals.ReportMessages.RptMsg_NoData);

                var broadcastReportLogging = new BroadcastReportLogging();
                if (!report.IsDotNetEnabled)
                {
                    //the .NET stuff will contain the pieces for doing the report log results, but for FoxPro we still need to handle it
                    broadcastReportLogging.UpdateReportLogResultsRecord(results, Globals.ReportLogKey, MasterDataStore);
                }

                broadcastReportLogging.UpdateReportLog(results, rptLogger, Globals.ReportLogKey);
            }
            catch (RecoverableSqlException)
            {
                //since we believe we can recover from this exception let's throw it up a level so the whole broadcast will get retried
                throw;
            }
            catch (ReportAbandonedException)
            {
                throw;
            }
            catch (Exception e)
            {
                var errorNumber = ErrorLogger.LogException(user.UserNumber, Globals.Agency, e, MethodBase.GetCurrentMethod(), ServerType.BroadcastServer, LOG);
                rptLogger.UpdateLog(Globals.ReportLogKey, ReportLogLogger.ReportStatus.SYSERROR);
                results.ReportRunSuccess = false;

                if (batch.send_error_email)
                {
                    new ErrorHandler().SendErrorEmail(LOG, batch, errorNumber, MasterDataStore, config, report);
                }
            }

            return results;
        }

        private void SetGlobalFixedDateCurrencyConversion()
        {
            var currencyFieldFunction = "CURRCONVFIXEDDATE";
            var getFixedDateCurrencyQuery = new GetClientExtrasByFieldFunctionAndAgencyQuery(MasterDataStore.MastersQueryDb, Globals.Agency, currencyFieldFunction);
            var currencyFieldData = getFixedDateCurrencyQuery.ExecuteQuery();
            Globals.FixedDateCurrencyConversion = !string.IsNullOrEmpty(currencyFieldData) && currencyFieldData == "YES";
        }
        
        private bool CheckUserReportRestrictions(int userNumber, int processId)
        {
            var getUser = new GetUserByUserNumberQuery(ClientDataStore.ClientQueryDb, userNumber);
            var user = getUser.ExecuteQuery();

            if (user == null)
            {
                ErrorLogger.LogError(userNumber, Globals.Agency, string.Format("User [{0}] does not exist.", userNumber),
                    MethodBase.GetCurrentMethod(), ServerType.BroadcastServer, LOG);
                return false;
            }

            if (!user.reports) return false;

            if (!user.agencyrpts)
            {
                var rptType = LookupFunctions.LookupReportType(processId, MasterDataStore);
                if (string.IsNullOrEmpty(rptType)) return false; //report doesn't exist

                if (rptType.EqualsIgnoreCase("AGENCY")) return false;
            }

            return true;
        }
        
    }
}
