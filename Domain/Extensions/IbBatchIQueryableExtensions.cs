using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;

namespace Domain.Extensions
{
    public static class IbBatchIQueryableExtensions
    {
        //public static IQueryable<bcstque4> ArePendingBroadcasts(this IQueryable<bcstque4> broadcasts)
        //{
        //    return broadcasts.Where(x => x.svrstatus.Equals(BroadcastCriteria.Pending, StringComparison.OrdinalIgnoreCase));
        //}

        public static IQueryable<ibbatch> AreNotEffectsBroadcasts(this IQueryable<ibbatch> broadcasts)
        {
            return broadcasts.Where(x => !x.outputdest.Equals(BroadcastCriteria.EffectsOutputDest, StringComparison.OrdinalIgnoreCase));
        }

        public static IQueryable<ibbatch> HaveBatchNumber(this IQueryable<ibbatch> broadcasts)
        {
            return broadcasts.Where(x => x.batchnum > 0);
        }

        public static IQueryable<ibbatch> AreNotStageAgencyBroadcasts(this IQueryable<ibbatch> broadcasts, string stageAgency)
        {
            return broadcasts.Where(x => !x.agency.Equals(stageAgency, StringComparison.OrdinalIgnoreCase));
        }

        public static IQueryable<ibbatch> AreNotLoggingAgencyBroadcasts(this IQueryable<ibbatch> broadcasts, IQueryable<broadcast_stage_agencies> loggingAgencies)
        {
            return broadcasts.Where(x => !loggingAgencies.Any(y => y.agency.Equals(x.agency, StringComparison.OrdinalIgnoreCase)
                                                                   && y.currently_staged
                                                                   && string.IsNullOrEmpty(y.staged_batchnumber)));
        }

        public static IEnumerable<ibbatch> AreNotSpecificBatchNumberLoggingBroadcasts(this IEnumerable<ibbatch> broadcasts, IEnumerable<broadcast_stage_agencies> loggingAgencies)
        {
            return broadcasts.Where(x => !loggingAgencies.Where(y => y.currently_staged
                                                                     && y.agency.Equals(x.agency, StringComparison.OrdinalIgnoreCase)
                                                                     && !string.IsNullOrEmpty(y.staged_batchnumber))
                .SelectMany(z => z.staged_batchnumber.Split(',', '|'))
                .Contains(x.batchnum.ToString())).ToList();
        }
    }
}
