using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using com.ciswired.libraries.CISLogger;

using CODE.Framework.Core.Utilities.Extensions;

using Domain.Exceptions;
using Domain.Helper;
using iBank.BroadcastServer.Utilities;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.ReportCriteriaHandlers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities;

using Domain.Orm.iBankMastersCommands;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared.Client;
using iBankDomain.Exceptions;
using Domain;
using iBank.Server.Utilities.Helpers;
using Domain.Services;

namespace iBank.BroadcastServer.BroadcastReport
{
    public class ServiceHandoff
    {
        private static readonly com.ciswired.libraries.CISLogger.ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public int FinalRecordsCount { get; set; }
        public string FoxProReportId { get; set; }

        private IMastersQueryable MasterQueryDb => new iBankMastersQueryable();
        

        public bool RunNetReport(ReportGlobals globals, LoadedListsParams loadedParams)
        {
            var buildWhere = new BuildWhere(new ClientFunctions()) { ReportGlobals = globals };
            
            var reportRunnerFactory = new ReportRunnerFactory(globals.ProcessKey, buildWhere, loadedParams);
            var reportRunner = reportRunnerFactory.Build();

            if (reportRunner == null) return false;

            var didReportRunSuccessfully = reportRunner.RunReport();
            FinalRecordsCount = reportRunner.GetFinalRecordsCount();

            return didReportRunSuccessfully;
        }

        public bool RunFoxProReport(bool savedReport, ref ReportGlobals globals, DateTime reportPeriodStartDate, DateTime reportPeriodEndDate, BroadcastServerFunction function)
        {
            var reportComplete = false;
            var reportId = Guid.NewGuid() + "_NET";
            FoxProReportId = reportId;

            AddReportHandoffRecords(reportId, globals, reportPeriodStartDate, reportPeriodEndDate);

            LOG.Info($"Waiting on FoxPro report server to complete report id [{reportId}]");

            //put a time limit on how long we will wait
            var serverNumber = ConfigurationManager.AppSettings["ServerNumber"].TryIntParse(0);
            var cache = new CacheService();
            int minutesToWait;
            if(Features.MinutesToWaitForFoxProToReturnToDB.IsEnabled())
            {
                minutesToWait = NETWaitForFoxProToReturnProperty.GetMinutesToWaitForFoxProToReturnForTheServer(cache, serverNumber, new iBankAdministrationQueryable());
            }
            else
            {
                minutesToWait = function == BroadcastServerFunction.LongRunning
                    ? 6 * 60
                    : ConfigurationManager.AppSettings["MinutesToWaitForFoxProToReturn"].TryIntParse(120);
            }
            var watch = Stopwatch.StartNew();
            
            while (!reportComplete)
            {
                if (watch.Elapsed.TotalMinutes > minutesToWait)
                {
                    throw new ReportAbandonedException("Timeout occurred while waiting for report to return from FoxPro server.");
                }
                
                var reportStatus = ReportHandoffRecordHandler.GetReportHandoffValue("REPORTSTATUS", reportId, MasterQueryDb);
                if (reportStatus.EqualsIgnoreCase("DONE"))
                {
                    watch.Stop();
                    reportComplete = true;
                    var returnText = ReportHandoffRecordHandler.GetReportHandoffValue("RETURNTEXT", reportId, MasterQueryDb);
                    globals.ReportInformation.ReturnCode = 1;
                    var href = ReportHandoffRecordHandler.GetReportHandoffValue("RETURNHREF", reportId, MasterQueryDb);
                    var code = ReportHandoffRecordHandler.GetReportHandoffValue("RETURNCODE", reportId, MasterQueryDb);
                    var returnCode = code.TryIntParse(2);
                    globals.ReportInformation.Href = href;
                    globals.ReportInformation.ReturnCode = returnCode;

                    if (!string.IsNullOrEmpty(returnText) && returnCode != 0)
                    {
                        if (RecoverableFoxProErrorHandler.IsRecoverableError(returnText, MasterQueryDb))
                        {
                            LOG.Warn("FoxPro NOLOCK error encountered. Setting up for retry.");
                            throw new RecoverableSqlException();
                        }

                        globals.ReportInformation.ReturnCode = 2;
                        globals.ReportInformation.ErrorMessage = returnText;
                        return false;
                    }
                    else
                    {
                        globals.ReportInformation.ReturnCode = returnCode;
                        globals.ReportInformation.ReturnText = returnText;
                    }
                    
                    if (string.IsNullOrEmpty(href))
                    {
                        globals.ReportInformation.ErrorMessage = "HREF not found!";
                        return false;
                    }
                    
                }
                else
                {
                    Thread.Sleep(5000);
                }
            }

            LOG.Info(string.Format("FoxPro report server finished processing report id [{0}]", reportId));

            return reportComplete;
        }

        private void AddReportHandoffRecords(string reportId, ReportGlobals globals, DateTime reportPeriodStartDate, 
            DateTime reportPeriodEndDate)
        {
            var handoffRecords = new List<reporthandoff>();
            var cfBox = ConfigurationManager.AppSettings["ColdFusionBox"];
            var dateCreated = DateTime.Now;
            var foxProRecordCreator = new FoxProReportHandoffRecordRetriever(new ReportHandoffRecordHandler(reportId, globals.UserLanguage, cfBox, globals.UserNumber, globals.Agency, dateCreated));
            
            handoffRecords.AddRange(foxProRecordCreator.GetGeneralParameters(globals.ProcessKey, globals.EProfileNumber));
            handoffRecords.AddRange(foxProRecordCreator.GetReportParameterRecordsExceptReportPeriodDates(globals.ReportParameters));
            handoffRecords.AddRange(foxProRecordCreator.GetReportPeriodDateRecords(reportPeriodStartDate, reportPeriodEndDate));

            handoffRecords.AddRange(foxProRecordCreator.GetAdvancedParameterRecords(globals.AdvancedParameters.Parameters, globals.AdvancedParameters.AndOr));
            handoffRecords.AddRange(foxProRecordCreator.GetMultiUdidRecords(globals.MultiUdidParameters.Parameters, globals.MultiUdidParameters.AndOr));

            var addReportHandoffRecordsCmd = new AddReportHandoffRecordCommand(new iBankMastersCommandDb(), handoffRecords);
            addReportHandoffRecordsCmd.ExecuteCommand();
        }
    }
}
