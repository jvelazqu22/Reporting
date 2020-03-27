using System;
using System.Linq;
using Domain.Helper;
using iBank.Entities.MasterEntities;

namespace Domain.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<bcstque4> AreFinishedBroadcasts(this IQueryable<bcstque4> broadcasts)
        {
            return broadcasts.Where(x => !x.svrstatus.Equals(BroadcastCriteria.Pending, StringComparison.OrdinalIgnoreCase))
                             .Where(x => !x.svrstatus.Equals(BroadcastCriteria.Running, StringComparison.OrdinalIgnoreCase));
        }

        public static IQueryable<bcstque4> AreErroredBroadcasts(this IQueryable<bcstque4> broadcasts)
        {
            return broadcasts.Where(x => x.errflag);
        }

        public static IQueryable<bcstque4> AreRunningBroadcasts(this IQueryable<bcstque4> broadcasts)
        {
            return broadcasts.Where(x => x.svrstatus.Equals(BroadcastCriteria.Running, StringComparison.OrdinalIgnoreCase));
        }

        public static IQueryable<bcstque4> ArePendingBroadcasts(this IQueryable<bcstque4> broadcasts)
        {
            return broadcasts.Where(x => x.svrstatus.Equals(BroadcastCriteria.Pending, StringComparison.OrdinalIgnoreCase));
        }

        public static IQueryable<bcstque4> HaveRunningTimeOverThreshold(this IQueryable<bcstque4> broadcasts, DateTime threshold)
        {
            return broadcasts.Where(x => x.starttime < threshold);
        }

        public static IQueryable<bcstque4> AreOnServer(this IQueryable<bcstque4> broadcasts, int serverNumber)
        {
            return broadcasts.Where(x => x.svrnumber == serverNumber);
        }

        public static IQueryable<bcstque4> AreEffectsBroadcasts(this IQueryable<bcstque4> broadcasts)
        {
            return broadcasts.Where(x => x.outputdest.Equals(BroadcastCriteria.EffectsOutputDest, StringComparison.OrdinalIgnoreCase));
        }

        public static IQueryable<bcstque4> AreNotEffectsBroadcasts(this IQueryable<bcstque4> broadcasts)
        {
            return broadcasts.Where(x => !x.outputdest.Equals(BroadcastCriteria.EffectsOutputDest, StringComparison.OrdinalIgnoreCase));
        }

        public static IQueryable<bcstque4> HaveBatchNumber(this IQueryable<bcstque4> broadcasts)
        {
            return broadcasts.Where(x => x.batchnum.HasValue);
        }

        public static IQueryable<bcstque4> AreStageAgencyBroadcasts(this IQueryable<bcstque4> broadcasts, string stageAgency)
        {
            return broadcasts.Where(x => x.agency.Equals(stageAgency, StringComparison.OrdinalIgnoreCase));
        }

        public static IQueryable<bcstque4> AreNotStageAgencyBroadcasts(this IQueryable<bcstque4> broadcasts, string stageAgency)
        {
            return broadcasts.Where(x => !x.agency.Equals(stageAgency, StringComparison.OrdinalIgnoreCase));
        }

        public static IQueryable<bcstque4> AreNotLoggingAgencyBroadcasts(this IQueryable<bcstque4> broadcasts, IQueryable<broadcast_stage_agencies> loggingAgencies)
        {
            return broadcasts.Where(x => !loggingAgencies.Any(y => y.agency.Equals(x.agency, StringComparison.OrdinalIgnoreCase)
                                                                    && y.currently_staged
                                                                    && string.IsNullOrEmpty(y.staged_batchnumber)));
        }

        public static IQueryable<bcstque4> AreLoggingAgencyBroadcasts(this IQueryable<bcstque4> broadcasts, IQueryable<broadcast_stage_agencies> loggingAgencies)
        {
            return broadcasts.Where(x => loggingAgencies.Any(y => y.agency.Equals(x.agency, StringComparison.OrdinalIgnoreCase)
                                                                   && y.currently_staged
                                                                   && string.IsNullOrEmpty(y.staged_batchnumber)));
        }

        public static IQueryable<bcstque4> AreOfflineBroadcasts(this IQueryable<bcstque4> broadcasts)
        {
            return broadcasts.Where(x => x.batchname.Length >= 7)
                             .Where(x => x.batchname.Substring(0, 7).Equals(BroadcastCriteria.OfflineRecord, StringComparison.OrdinalIgnoreCase));
        }
    }
}
