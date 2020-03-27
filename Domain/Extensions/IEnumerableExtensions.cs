using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Orm.Classes;
using iBank.Entities.MasterEntities;

namespace Domain.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<int> GetCorrespondingServerNumbers(this IEnumerable<BroadcastServerConfiguration> serverConfigs, BroadcastServerFunction function)
        {
            return serverConfigs.Where(x => x.Function == function)
                                .Select(y => y.ServerNumber);
        }

        public static IEnumerable<bcstque4> AreNotSpecificBatchNumberLoggingBroadcasts(this IEnumerable<bcstque4> broadcasts, IEnumerable<broadcast_stage_agencies> loggingAgencies)
        {
            return broadcasts.Where(x => !loggingAgencies.Where(y => y.currently_staged
                                                                              && y.agency.Equals(x.agency, StringComparison.OrdinalIgnoreCase)
                                                                              && !string.IsNullOrEmpty(y.staged_batchnumber))
                                                        .SelectMany(z => z.staged_batchnumber.Split(',', '|'))
                                                        .Contains(x.batchnum.ToString())).ToList();
        }

        public static IEnumerable<bcstque4> AreSpecificBatchNumberLoggingBroadcasts(this IEnumerable<bcstque4> broadcasts, IEnumerable<broadcast_stage_agencies> loggingAgencies)
        {
            return broadcasts.Where(x => loggingAgencies.Where(y => y.currently_staged
                                                                     && y.agency.Equals(x.agency, StringComparison.OrdinalIgnoreCase)
                                                                     && !string.IsNullOrEmpty(y.staged_batchnumber))
                                                        .SelectMany(z => z.staged_batchnumber.Split(',', '|'))
                                                        .Contains(x.batchnum.ToString())).ToList();
        }

        public static IEnumerable<bcstque4> AreNotLongRunningBroadcasts(this IEnumerable<bcstque4> broadcasts, IList<bcstque4> longRunningBroadcasts)
        {
            var longRunningSequenceNums = longRunningBroadcasts.Select(x => x.bcstseqno);

            return broadcasts.Where(x => !longRunningSequenceNums.Contains(x.bcstseqno));
        }

        public static IEnumerable<bcstque4> AreStageAgencyBroadcasts(this IEnumerable<bcstque4> broadcasts, string stageAgency)
        {
            return broadcasts.Where(x => x.agency.Equals(stageAgency, StringComparison.OrdinalIgnoreCase));
        }
    }
}
