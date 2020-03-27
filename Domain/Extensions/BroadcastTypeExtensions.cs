using Domain.Constants;
using Domain.Helper;
using iBank.Entities.MasterEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Extensions
{
    public static class BroadcastTypeExtensions
    {
        public static bool IsPendingBroadcast(this bcstque4 bcst)
        {
            return bcst.svrstatus.Equals(BroadcastCriteria.Pending, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsEffectsBroadcast(this bcstque4 bcst)
        {
            return bcst.outputdest.Equals(BroadcastCriteria.EffectsOutputDest, StringComparison.OrdinalIgnoreCase);
        }

        public static bool HasBatchNumber(this bcstque4 bcst)
        {
            return bcst.batchnum.HasValue;
        }

        public static bool IsStageAgencyBroadcast(this bcstque4 bcst)
        {
            return bcst.agency.Trim().Equals(Broadcasts.StageAgency, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsLoggingAgencyBroadcast(this bcstque4 bcst, IList<broadcast_stage_agencies> loggingAgencies)
        {
            return loggingAgencies.Any(y => y.agency.Equals(bcst.agency, StringComparison.OrdinalIgnoreCase)
                                                                   && y.currently_staged
                                                                   && string.IsNullOrEmpty(y.staged_batchnumber));
        }

        public static bool IsOfflineBroadcast(this bcstque4 bcst)
        {
            return bcst.batchname.Length >= 7 && bcst.batchname.Substring(0, 7).Equals(BroadcastCriteria.OfflineRecord, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsSpecificBatchNumberLoggingBroadcast(this bcstque4 bcst, IList<broadcast_stage_agencies> loggingAgencies)
        {
            return loggingAgencies.Where(y => y.currently_staged
                                            && y.agency.Trim().Equals(bcst.agency.Trim(), StringComparison.OrdinalIgnoreCase)
                                            && !string.IsNullOrEmpty(y.staged_batchnumber))
                            .ToList()
                            .SelectMany(z => z.staged_batchnumber.Split(',', '|'))
                            .Contains(bcst.batchnum.ToString());
        }

        public static bool IsHotType(this bcstque4 bcst, IList<broadcast_stage_agencies> loggingAgencies)
        {
            return IsPendingBroadcast(bcst) &&
                IsEffectsBroadcast(bcst) &&
                HasBatchNumber(bcst) &&
                !IsStageAgencyBroadcast(bcst) &&
                !IsLoggingAgencyBroadcast(bcst, loggingAgencies) &&
                !IsSpecificBatchNumberLoggingBroadcast(bcst, loggingAgencies);
        }

        public static bool IsPrimaryType(this bcstque4 bcst, IList<broadcast_stage_agencies> loggingAgencies)
        {
            return IsPendingBroadcast(bcst) &&
                !IsEffectsBroadcast(bcst) &&
                HasBatchNumber(bcst) &&
                !IsStageAgencyBroadcast(bcst) &&
                !IsLoggingAgencyBroadcast(bcst, loggingAgencies) &&
                !IsSpecificBatchNumberLoggingBroadcast(bcst, loggingAgencies);
        }

        public static bool IsOfflineType(this bcstque4 bcst, IList<broadcast_stage_agencies> loggingAgencies)
        {
            return IsPendingBroadcast(bcst) &&
                IsOfflineBroadcast(bcst) &&
                HasBatchNumber(bcst) &&
                !IsStageAgencyBroadcast(bcst) &&
                !IsLoggingAgencyBroadcast(bcst, loggingAgencies) &&
                !IsSpecificBatchNumberLoggingBroadcast(bcst, loggingAgencies);
        }

        public static bool IsStageType(this bcstque4 bcst)
        {
            return IsPendingBroadcast(bcst) &&
                HasBatchNumber(bcst) &&
                IsStageAgencyBroadcast(bcst);
        }

        public static bool IsLoggingType(this bcstque4 bcst, IList<broadcast_stage_agencies> loggingAgencies)
        {
            return IsLoggingAgencyBroadcast(bcst, loggingAgencies) ||
                IsSpecificBatchNumberLoggingBroadcast(bcst, loggingAgencies);
        }

        public static bool IsLongrunningType(this bcstque4 bcst, IList<broadcast_stage_agencies> loggingAgencies, Dictionary<string, int> thresholds, int defaultThreshold)
        {
            if (IsPrimaryType(bcst, loggingAgencies) || IsOfflineBroadcast(bcst))
            {
                var actualRange = 0;
                if (bcst.runspcl)
                {
                    actualRange = DateRangeHelper.GetSpecialDateRangeInterval(bcst);
                }
                else
                {
                    actualRange = DateRangeHelper.GetActualDateRangeInterval(bcst);
                }

                if (!thresholds.TryGetValue(bcst.agency.Trim(), out var rangeLimit))
                {
                    rangeLimit = defaultThreshold;
                }

                return actualRange >= rangeLimit;
            }
            return false;
        }

    }
}
