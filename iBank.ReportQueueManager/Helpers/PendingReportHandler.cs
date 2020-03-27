using System;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Orm.iBankMastersCommands.PendingRecords;
using Domain.Orm.iBankMastersCommands.Report;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Helpers;

namespace iBank.ReportQueueManager.Helpers
{
    public class PendingReportHandler
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private PendingReports _pendingReport;

        public PendingReportHandler(PendingReports pending)
        {
            _pendingReport = pending;
        }

        public void SetResponsibleApp(IMasterDataStore store, IConfigurationWrapper wrapper)
        {
            if (wrapper.IsDevVersion || IsDemoUser(wrapper))
            {
                _pendingReport.IsDotNet = true;
                return;
            }

            var validation = new ReportValidation(store);
            _pendingReport.IsDotNet = validation.IsReportConvertedAndAgencyEnabled(_pendingReport.ReportId, _pendingReport.UserNumber);
        }

        private bool IsDemoUser(IConfigurationWrapper wrapper)
        {
            if (!_pendingReport.Agency.Equals("DEMO", StringComparison.OrdinalIgnoreCase)) return false;
            
            return wrapper.DemoUsers.Contains(_pendingReport.UserNumber);
        }

        public void AddToPendingReportTable(DateTime now, IMasterDataStore store)
        {
            var pendingReport = InsertNewPendingReportIntoQueue(_pendingReport, now, store);
            if (pendingReport != null) InsertNewReportIntoReportHistory(pendingReport, now, store);
        }

        private readonly int _primaryKeyException = 2627;
        private PendingReports InsertNewPendingReportIntoQueue(PendingReports pendingReport, DateTime now, IMasterDataStore store)
        {
            pendingReport.TimeStamp = now;
            try
            {
                var cmd = new AddPendingReportCommand(store.MastersCommandDb, pendingReport);
                cmd.ExecuteCommand();

                var reportType = pendingReport.IsDotNet ? ".NET" : "FoxPro";
                LOG.Debug($"Added reportid [{pendingReport.ReportId}] to pending_reports as a [{reportType}] report.");
                return pendingReport;
            }
            catch (Exception e)
            {
                if (e.GetBaseException() is SqlException sqlEx && sqlEx.Number == _primaryKeyException)
                {
                    //do nothing: swallow primary key exception as there is a race condition here which is ok
                }
                else throw;
            }

            return null;
        }

        private void InsertNewReportIntoReportHistory(PendingReports pendingReport, DateTime now, IMasterDataStore store)
        {
            try
            {
                var reportHistory = new ReportHistory()
                {
                    run_btn_clicked_on = pendingReport.ReportHandOffDateCreated,
                    created_on = now,
                    reportid = pendingReport.ReportId,
                };
                LOG.Debug($"New record added to the report history: report id [{pendingReport.ReportId}]");
                var cmd = new AddReportHistoryRecordCommand(store.MastersCommandDb, reportHistory);
                cmd.ExecuteCommand();
            }
            catch (Exception ex)
            {
                LOG.Error($"Failed to add record into report history table report id: {pendingReport.ReportId}", ex);
            }
        }
    }
}

