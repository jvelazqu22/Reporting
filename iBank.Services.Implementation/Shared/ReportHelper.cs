using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using com.ciswired.libraries.CISLogger;
using Domain.Orm.Classes;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Logging;
using iBank.Server.Utilities.ReportCriteriaHandlers;
using Domain.Orm.iBankMastersCommands;
using Domain.Orm.iBankMastersCommands.PendingRecords;
using Domain.Orm.iBankMastersQueries.Other;
using Domain.Orm.iBankMastersQueries.ReportServer;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.Shared
{
    public static class ReportHelper
    {
        private static readonly com.ciswired.libraries.CISLogger.ILogger LOG = new LogManagerWrapper().GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ReportLogLogger _reportLogLogger = new ReportLogLogger();

        /// <summary>
        /// The report was successful. Make all database changes necessary. 
        /// </summary>
        public static void EndReport(PendingReportInformation report, ReportGlobals globals)
        {
            var recordsToAdd = new List<reporthandoff>();
            var creator = new ReportHandoffRecordHandler(report.ReportId, " ", report.ColdFusionBox, report.UserNumber, report.Agency);
            
            recordsToAdd.Add(creator.CreateReportHandoffRecord(false, "ReturnCode", report.ReturnCode.ToString(CultureInfo.InvariantCulture)));
            recordsToAdd.Add(creator.CreateReportHandoffRecord(false, "ReturnText", globals.IsEffectsDelivery ? globals.ReportInformation.ReturnText : ""));

            if (!globals.IsEffectsDelivery)
            {
                recordsToAdd.Add(creator.CreateReportHandoffRecord(false, "ReturnHRef", report.Href));
                recordsToAdd.Add(creator.CreateReportHandoffRecord(false, "ReportFileName", Path.GetFileName(report.Href)));
            }
            
            //add records to reporthandoff
            var cmd = new AddReportHandoffRecordCommand(new iBankMastersCommandDb(), recordsToAdd);
            cmd.ExecuteCommand();

            //FOR REPORTING TRAVET NOTIFICATION CENTER
            if (report.ReportLogKey != 0)
            {
                SetReportLogResults(report, 0, string.Empty, report.Href);
            }

            UpdateReportStatusToDone(report);
            
            _reportLogLogger.UpdateLog(report.ReportLogKey, ReportLogLogger.ReportStatus.SUCCESS, report.RecordCount);

            DeleteRunningReport(report);
        }

        private static void DeleteRunningReport(PendingReportInformation report)
        {
            //Remove the "running report" record so the web tool knows to let the user run it again
            var runningReport = new GetRunningReportQuery(new iBankMastersQueryable(), report.ReportLogKey).ExecuteQuery();
            if (runningReport != null)
            {
                var deleteRunningReport = new RemoveReportFromRunningReportsCommand(new iBankMastersCommandDb(), runningReport);
                deleteRunningReport.ExecuteCommand();
            }

            var runningPendingReportToDelete = new GetPendingReportByIdQuery(new iBankMastersQueryable(), report.ReportId).ExecuteQuery();
            if (runningPendingReportToDelete != null)
            {
                try
                {
                    new RemovePendingReportsCommand(new iBankMastersCommandDb(), runningPendingReportToDelete).ExecuteCommand();
                }
                catch (Exception)
                {
                    // Ignore exception. It is possible that the ReportQueueManager deleted it first.
                }
            }
            else
            {
                LOG.Warn($"report [{report.ReportId}] was not found to be deleted.");
            }
        }

        /// <summary>
        /// When a report fails, two records are created in the report handoff table, and the report status is set to done. 
        /// </summary>
        /// <param name="report"></param>
        public static void PostError(PendingReportInformation report)
        {
            var recsToAdd = new List<reporthandoff>();
            var creator = new ReportHandoffRecordHandler(report.ReportId, "", report.ColdFusionBox, report.UserNumber, report.Agency);
            
            recsToAdd.Add(creator.CreateReportHandoffRecord(false, "ReturnCode", report.ReturnCode.ToString(CultureInfo.InvariantCulture)));
            recsToAdd.Add(creator.CreateReportHandoffRecord(false, "ReturnText", report.ErrorMessage));

            var cmd = new AddReportHandoffRecordCommand(new iBankMastersCommandDb(), recsToAdd);
            cmd.ExecuteCommand();
            
            //FOR REPORTING TRAVET NOTIFICATION CENTER
            if (report.ReportLogKey != 0)
            {
                SetReportLogResults(report, report.ReturnCode, report.ErrorMessage.Left(150), string.Empty);
            }

            UpdateReportStatusToDone(report);

            DeleteRunningReport(report);
        }

        private static void UpdateReportStatusToDone(PendingReportInformation report)
        {
            var query = new GetReportStatusHandoffRecordQuery(new iBankMastersQueryable(), report.ReportId, report.Agency,
                report.UserNumber, report.ColdFusionBox);
            var rec = query.ExecuteQuery();

            if (rec == null) return;

            rec.parmvalue = "DONE";
            var updateReportHandoff = new UpdateReportHandoffRecordCommand(new iBankMastersCommandDb(), rec);
            updateReportHandoff.ExecuteCommand();
            LOG.Info($"report [{report.ReportId}] handoff record updated to DONE");
        }

        private static void SetReportLogResults(PendingReportInformation report, int returnCode, string errorMessage, string reportUrl)
        {
            var logger = new ReportLogResultsLogger();
            var rptLogResult = new GetReportLogResultsByLogKeyQuery(new iBankMastersQueryable(), report.ReportLogKey).ExecuteQuery();

            if (rptLogResult == null)
            {
                logger.Create(report.ReportLogKey, reportUrl, errorMessage, returnCode, new iBankMastersCommandDb());
            }
            else
            {
                logger.Update(rptLogResult, reportUrl, errorMessage, returnCode, new iBankMastersCommandDb());
            }
        }

        /// <summary>
        /// Creates an origin or dest code from the ibmktsegs table that matches the 
        /// old code from the iblegs table.
        /// </summary>
        /// <param name="inPort"></param>
        /// <param name="mode"></param>
        /// <param name="airline"></param>
        /// <returns></returns>
        public static string CreateOriginDestCode(string inPort, string mode, string airline)
        {
            var rrStationNumber = 0;
            var railRoadLookup = false;
            railRoadLookup = int.TryParse(inPort, out rrStationNumber);

            if (!railRoadLookup && mode.EqualsIgnoreCase("R") && !inPort.Contains("-")) inPort = inPort.Trim() + "-" + airline.Trim();
            return inPort;
        }
    }
}
