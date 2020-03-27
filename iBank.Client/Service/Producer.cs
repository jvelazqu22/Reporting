using com.ciswired.libraries.CISLogger;
using iBank.Server.Utilities.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Reflection;
using System.Threading;
using Domain.Models.OnlineReportServer;
using Domain.Orm.Classes;
using Domain.Orm.iBankMastersCommands.Broadcast;
using Domain.Orm.iBankMastersCommands.Report;
using Domain.Orm.iBankMastersQueries.ReportServer;
using iBank.Entities.MasterEntities;
using iBank.ReportServer.Helpers;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.Exceptions;

namespace iBank.ReportServer.Service
{
    public class Producer
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly IReportServerLogger RPT_LOG = new ReportServerLogger(new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType));

        public void LoadReportsToRun(MaintenanceModeState maintState, Parameters p)
        {
            while (!p.IsMaintenanceModeRequested)
            {
                try
                {
                    var pendingReports = GetPendingReports(p);

                    AttemptToAddReports(pendingReports, p.ReportsToExecute, p.MasterDataStore);

                    CheckMaintenanceModeStatus(p, maintState);

                    Thread.Sleep(2000);
                }
                catch (EntityCommandExecutionException cmdEx)
                {
                    try
                    {
                        EFExceptionHandling.HandleEntityCommandExecutionException(cmdEx);
                    }
                    catch (RecoverableSqlException sqlEx)
                    {
                        LOG.Warn(sqlEx.ToString(), sqlEx);
                    }
                    catch (Exception ex)
                    {
                        p.ReportsToExecute.CompleteAdding();
                        LOG.Error(ex.ToString(), ex);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    p.ReportsToExecute.CompleteAdding();
                    LOG.Error(ex.ToString(), ex);
                    break;
                }
            }
        }

        private IEnumerable<PendingReportInformation> GetPendingReports(Parameters p)
        {
            var retriever = new PendingReportsRetriever();
            return retriever.GetPendingReports(p.MasterDataStore, p.DemoUsers, p.ServerNumber, p.IsDevMode, p.ServerFunction);
        }

        private void AttemptToAddReports(IEnumerable<PendingReportInformation> pendingReports, BlockingCollection<PendingReportInformation> reportsToExecute, IMasterDataStore store)
        {
            foreach (var report in pendingReports)
            {
                if (report != null) // per logs, sometimes report is null
                {
                    if (reportsToExecute.All(x => x.ReportId != report.ReportId))
                    {
                        RPT_LOG.DebugLogWithReportInfo(report, "Picked up pending report.");

                        UpdateReportHistoryRecord(report.ReportId, store);

                        reportsToExecute.Add(report);

                        Thread.Sleep(500);
                    }
                }
            }
        }

        private void UpdateReportHistoryRecord(string reportId, IMasterDataStore store)
        {
            try
            {
                var recordToUpdate = new GetReportHistoryByIdQuery(store.MastersQueryDb, reportId).ExecuteQuery();
                if (recordToUpdate != null)
                {
                    recordToUpdate.added_to_pool = DateTime.Now;
                    var updateBroadcastHistoryCommand = new UpdateReportHistoryRecordCommand(store.MastersCommandDb, recordToUpdate);
                    updateBroadcastHistoryCommand.ExecuteCommand();
                }
            }
            catch (Exception e)
            {
                LOG.Debug("Attempt to update broadcast history record failed.", e);
            }
        }

        private void CheckMaintenanceModeStatus(Parameters p, MaintenanceModeState maintState)
        {
            p.IsMaintenanceModeRequested = maintState.IsMaintenanceModeRequested();

            if (p.IsMaintenanceModeRequested)
            {
                LOG.Info("Maintenance mode requested. No longer adding pending reports.");
                p.ReportsToExecute.CompleteAdding();
            }
        }

        // this one is used when running server from local machine in debug mode
        public void LoadReportsDebugVersion(MaintenanceModeState maintenanceModeState, Parameters p)
        {
            try
            {
                var pendingReports = GetPendingReports(p);
                AttemptToAddReports(pendingReports, p.ReportsToExecute, p.MasterDataStore);

                CheckMaintenanceModeStatus(p, maintenanceModeState);

                Thread.Sleep(5000);
            }
            catch (EntityCommandExecutionException cmdEx)
            {
                try
                {
                    EFExceptionHandling.HandleEntityCommandExecutionException(cmdEx);
                }
                catch (RecoverableSqlException sqlEx)
                {
                    LOG.Warn(sqlEx.ToString(), sqlEx);
                }
                catch (Exception ex)
                {
                    p.ReportsToExecute.CompleteAdding();
                    LOG.Error(ex.ToString(), ex);
                }
            }
            catch (Exception ex)
            {
                p.ReportsToExecute.CompleteAdding();
                LOG.Error(ex.ToString(), ex);
            }
        }
    }
    
}
