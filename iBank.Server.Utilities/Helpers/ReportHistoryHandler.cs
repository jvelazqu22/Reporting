using System;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Orm.iBankMastersCommands.Report;
using Domain.Orm.iBankMastersQueries.BroadcastQueries.BroadcastHistoryQueries;
using Domain.Orm.iBankMastersQueries.ReportServer;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Server.Utilities.Helpers
{
    public class ReportHistoryHandler
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void InsertNewBroadcastReportIntoReportHistory(Guid reportId, DateTime now, IMasterDataStore store, bcstque4 broadcast)
        {
            try
            {
                var broadcastHistoryRec = new GetLastBroadcastHistoryByBatchNumberQuery(store.MastersQueryDb, broadcast).ExecuteQuery();
                var reportHistory = new ReportHistory()
                {
                    start_of_run = now,
                    reportid = reportId.ToString(),
                    batchname = broadcast.batchname,
                    batchnum = broadcast.batchnum,
                };

                if (broadcastHistoryRec != null) reportHistory.broadcast_history_id = broadcastHistoryRec.Id;

                LOG.Debug($"New braodcast record added to the report history: report id [{reportId}]");
                var cmd = new AddReportHistoryRecordCommand(store.MastersCommandDb, reportHistory);
                cmd.ExecuteCommand();
            }
            catch (Exception ex)
            {
                LOG.Error($"Failed to add record into report history table report id: {reportId}", ex);
            }
        }


        public void UpdateReportHistoryStartRun(string reportId, IMasterDataStore store)
        {
            try
            {
                var recordToUpdate = new GetReportHistoryByIdQuery(store.MastersQueryDb, reportId).ExecuteQuery();
                if (recordToUpdate != null)
                {
                    recordToUpdate.start_of_run = DateTime.Now;
                    var updateBroadcastHistoryCommand = new UpdateReportHistoryRecordCommand(store.MastersCommandDb, recordToUpdate);
                    updateBroadcastHistoryCommand.ExecuteCommand();
                }
            }
            catch (Exception e)
            {
                LOG.Debug("Attempt to update report history start of run record failed.", e);
            }
        }

        public void UpdateReportHistoryFinishedRun(string reportId, IMasterDataStore store, int finalRecords = 0)
        {
            try
            {
                var recordToUpdate = new GetReportHistoryByIdQuery(store.MastersQueryDb, reportId).ExecuteQuery();
                if (recordToUpdate != null)
                {
                    recordToUpdate.finished_run = DateTime.Now;
                    recordToUpdate.number_of_records = finalRecords;
                    var updateBroadcastHistoryCommand = new UpdateReportHistoryRecordCommand(store.MastersCommandDb, recordToUpdate);
                    updateBroadcastHistoryCommand.ExecuteCommand();
                }
            }
            catch (Exception e)
            {
                LOG.Debug("Attempt to update report history finished run record failed.", e);
            }
        }

        public void UpdateReportHistoryFinishedRunAndReplaceFakeReportIdWithActualReportName(string reportId, IMasterDataStore store, int finalRecords, string reportName)
        {
            try
            {
                var recordToUpdate = new GetReportHistoryByIdQuery(store.MastersQueryDb, reportId).ExecuteQuery();
                if (recordToUpdate != null)
                {
                    if(!string.IsNullOrEmpty(reportName)) recordToUpdate.reportid = reportName;
                    recordToUpdate.finished_run = DateTime.Now;
                    recordToUpdate.number_of_records = finalRecords;
                    var updateBroadcastHistoryCommand = new UpdateReportHistoryRecordCommand(store.MastersCommandDb, recordToUpdate);
                    updateBroadcastHistoryCommand.ExecuteCommand();
                }
            }
            catch (Exception e)
            {
                LOG.Debug("Attempt to update report history finished run record failed.", e);
            }
        }

    }
}
