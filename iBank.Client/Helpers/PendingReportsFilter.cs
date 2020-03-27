using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Helper;
using Domain.Orm.Classes;
using Domain.Orm.iBankMastersQueries.Agency;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.ReportServer.Helpers
{
    public class PendingReportsFilter
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public IMasterDataStore MasterDataStore { get; }

        public PendingReportsFilter(IMasterDataStore masterDataStore)
        {
            MasterDataStore = masterDataStore;
        }

        public IList<PendingReportInformation> GetFilteredReports(List<PendingReportInformation> pendingReports, ReportServerFunction function, bool isDevMode)
        {
            //don't filter for local development
            if (isDevMode) return pendingReports;

            var query = new GetReportServerStageRecordsQuery(MasterDataStore.MastersQueryDb);
            var stage = query.ExecuteQuery();
            
            switch (function)
            {
                case ReportServerFunction.Primary:
                    if(stage.Any())
                    {
                        FilterOutStagedRecords(pendingReports, stage);
                    }
                    break;
                case ReportServerFunction.Stage:
                    if (!stage.Any()) //if no one is on stage then there is nothing for this server to run
                    {
                        return new List<PendingReportInformation>();
                    }
                    else
                    {
                        pendingReports = GetStagedRecords(pendingReports, stage);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(function), function, null);
            }

            return pendingReports;
        }

        private void FilterOutStagedRecords(List<PendingReportInformation> pendingReports, IList<report_server_stage> stage)
        {
            var agenciesOnStage = stage.Where(x => !string.IsNullOrEmpty(x.agency))
                                        .Select(x => x.agency?.ToUpper().Trim())
                                        .ToList();

            var dbsOnStage = stage.Where(x => !string.IsNullOrEmpty(x.database_name))
                                    .Select(x => x.database_name?.ToUpper().Trim())
                                    .ToList();

            if (dbsOnStage.Any())
            {
                //get all the agencies that are on that database
                var dbStageQuery = new GetAgenciesOnDatabaseQuery(MasterDataStore.MastersQueryDb, dbsOnStage);
                var agencies = dbStageQuery.ExecuteQuery();
                agenciesOnStage.AddRange(agencies);
            }

            if (agenciesOnStage.Any())
            {
                RemoveStagedAgencies(pendingReports, agenciesOnStage);
            }
        }

        private List<PendingReportInformation> GetStagedRecords(List<PendingReportInformation> pendingReports, IList<report_server_stage> stage)
        {
            var stagedReports = new List<PendingReportInformation>();
            var agenciesOnStage = stage.Where(x => !string.IsNullOrEmpty(x.agency))
                                                   .Select(x => x.agency.Trim())
                                                   .ToList();
            var dbsOnStage = stage.Where(x => !string.IsNullOrEmpty(x.database_name))
                                  .Select(x => x.database_name.Trim())
                                  .ToList();

            if (dbsOnStage.Any())
            {
                //get all the agencies that are on a staged database
                var dbStageQuery = new GetAgenciesOnDatabaseQuery(MasterDataStore.MastersQueryDb, dbsOnStage);
                var agencies = dbStageQuery.ExecuteQuery();

                agenciesOnStage.AddRange(agencies);
            }

            if (agenciesOnStage.Any())
            {
                stagedReports = RetrieveStagedAgencyReports(pendingReports, agenciesOnStage);
            }

            return stagedReports;
        }

        public void RemoveStagedAgencies(List<PendingReportInformation> pendingReports, IList<string> agenciesOnStage)
        {
            if (pendingReports == null || agenciesOnStage == null) return;
            if (!pendingReports.Any() || !agenciesOnStage.Any()) return;

            foreach (var rec in pendingReports.ToList())
            {
                if (rec == null) continue;
                try
                {
                    if (agenciesOnStage.Contains(rec.Agency?.Trim()))
                    {
                        pendingReports.Remove(rec);
                    }
                }
                catch (Exception ex1)
                {
                    try
                    {
                        var stageAgencies = string.Join(",", agenciesOnStage);
                        var msg = $"error on rec with rec.Agency: [{rec.Agency ?? ""}] rec.ReportId: [{rec.ReportId ?? ""}] " +
                                  $"rec.ServerNumber: {rec.ServerNumber} rec.UserNumber: {rec.UserNumber} " +
                                  $"agenciesOnStage: {stageAgencies}";
                        LOG.Error(msg, ex1);
                    }
                    catch (Exception ex2)
                    {
                        LOG.Error("ex1", ex1);
                        LOG.Error("ex2", ex2);
                    }
                }
            }
        }

        private List<PendingReportInformation> RetrieveStagedAgencyReports(List<PendingReportInformation> pendingReports, IList<string> agenciesOnStage)
        {
            var stagedPendingReports = new List<PendingReportInformation>();

            if (pendingReports == null || agenciesOnStage == null) return stagedPendingReports;
            if (!pendingReports.Any() || !agenciesOnStage.Any()) return stagedPendingReports;

            foreach (var rec in pendingReports.ToList())
            {
                if (rec == null) continue;
                try
                {
                    if (agenciesOnStage.Contains(rec.Agency?.Trim()))
                    {
                        stagedPendingReports.Add(rec);
                    }
                }
                catch (Exception ex1)
                {
                    try
                    {
                        var stageAgencies = string.Join(",", agenciesOnStage);
                        var msg = $"error on rec with rec.Agency: [{rec.Agency ?? "[null]"}] rec.ReportId: [{rec.ReportId ?? ""}] " +
                                  $"rec.ServerNumber: {rec.ServerNumber} rec.UserNumber: {rec.UserNumber} " +
                                  $"agenciesOnStage: {stageAgencies}";
                        LOG.Error(msg, ex1);
                    }
                    catch (Exception ex2)
                    {
                        LOG.Error("ex1", ex1);
                        LOG.Error("ex2", ex2);
                    }
                }
            }

            return stagedPendingReports;
            //return pendingReports.Where(x => agenciesOnStage.Contains(x.Agency?.Trim())).ToList();
        }
    }
    
}
