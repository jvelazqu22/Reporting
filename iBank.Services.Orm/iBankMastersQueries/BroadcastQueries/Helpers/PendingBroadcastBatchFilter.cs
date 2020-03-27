using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

namespace iBank.Services.Orm.iBankMastersQueries.BroadcastQueries.Helpers
{
    public class PendingBroadcastBatchFilter
    {
        public List<bcstque4> GetPrimaryServerBatches(IQueryable<bcstque4> broadcastQueue, IQueryable<broadcast_stage_agencies> broadcastStageAgencies,
            IQueryable<broadcast_long_running_agencies> longRunningAgencies)
        {
            var batches = GetBatches(broadcastQueue, longRunningAgencies).Where(x => x.outputdest != BroadcastCriteria.EffectsOutputDest).ToList();

            return batches.Except(GetStagedRecords(batches, broadcastStageAgencies)).ToList();
        }

        public List<bcstque4> GetOfflineServerBatches(IQueryable<bcstque4> broadcastQueue, IQueryable<broadcast_stage_agencies> broadcastStageAgencies,
                                                       IQueryable<broadcast_long_running_agencies> longRunningAgencies)
        {
            var batches = GetBatches(broadcastQueue, longRunningAgencies).Where(x => x.batchname.Length >= 7
                                                            && x.batchname.Substring(0, 7).Equals(BroadcastCriteria.OfflineRecord,
                                                                                                StringComparison.OrdinalIgnoreCase)).ToList();

            return batches.Except(GetStagedRecords(batches, broadcastStageAgencies)).ToList();
        }

        public List<bcstque4> GetHotServerBatches(IQueryable<bcstque4> broadcastQueue, IQueryable<broadcast_stage_agencies> broadcastStageAgencies,
                                                   IQueryable<broadcast_long_running_agencies> longRunningAgencies)
        {
            var batches = GetBatches(broadcastQueue, longRunningAgencies).Where(x => x.outputdest == BroadcastCriteria.EffectsOutputDest).ToList();

            return batches.Except(GetStagedRecords(batches, broadcastStageAgencies)).ToList();
        }

        public List<bcstque4> GetStageServerBatches(IQueryable<bcstque4> broadcastQueue, IQueryable<broadcast_stage_agencies> broadcastStageAgencies)
        {
            var batches = broadcastQueue.Where(bsq => broadcastStageAgencies.Any(bsa => bsa.agency.Trim() == bsq.agency.Trim() && bsa.currently_staged)
                                                            && bsq.svrstatus.Trim().Equals(BroadcastCriteria.Pending, StringComparison.OrdinalIgnoreCase))
                                               .ToList();

            return broadcastStageAgencies.Any(x => !string.IsNullOrEmpty(x.staged_batchnumber)) 
                ? GetStagedRecords(batches, broadcastStageAgencies) 
                : batches;
        }

        public List<bcstque4> GetLongRunningServerBatches(IQueryable<bcstque4> broadcastQueue, IQueryable<broadcast_long_running_agencies> longRunningAgencies)
        {
            return broadcastQueue.Where(bsq => longRunningAgencies.Any(lra => lra.agency.Trim() == bsq.agency.Trim() && lra.currently_processing_long_running)
                                                         && bsq.svrstatus.Trim().Equals(BroadcastCriteria.Pending, StringComparison.OrdinalIgnoreCase))
                                                .ToList();
        }

        private static IEnumerable<bcstque4> GetBatches(IQueryable<bcstque4> broadcastQueue, IQueryable<broadcast_long_running_agencies> longRunningAgencies)
        {
            return broadcastQueue.Where(bsq => !longRunningAgencies.Any(lra => lra.agency.Trim() == bsq.agency.Trim() && lra.currently_processing_long_running)
                                                && bsq.svrstatus.Trim().Equals(BroadcastCriteria.Pending, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private static List<bcstque4> GetStagedRecords(List<bcstque4> batches, IQueryable<broadcast_stage_agencies> broadcastStageAgencies)
        {
            var batchesWithAgencyOnStage = batches.Where(x => broadcastStageAgencies.Any(bsa => bsa.agency.Trim() == x.agency.Trim() && bsa.currently_staged)).ToList();
            var stagedRecords = new List<bcstque4>();

            foreach (var batch in batchesWithAgencyOnStage)
            {
                var stageAgency = broadcastStageAgencies.FirstOrDefault(x => x.agency.Trim() == batch.agency.Trim());

                if (stageAgency == null) continue;

                if (string.IsNullOrEmpty(stageAgency.staged_batchnumber))
                {
                    stagedRecords.Add(batch);
                }
                else
                {
                    var batchNumbersToStage = stageAgency.staged_batchnumber.Split(',').Select(int.Parse).ToList();

                    if (batch.batchnum.HasValue && batchNumbersToStage.Contains(batch.batchnum.Value)) stagedRecords.Add(batch);
                }
            }

            return stagedRecords;
        }
    }
}
