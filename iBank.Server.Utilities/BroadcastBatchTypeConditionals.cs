using System;

using Domain.Helper;

namespace iBank.Server.Utilities
{
    public static class BroadcastBatchTypeConditionals
    {
        public static bool IsBatchRunOffline(string batchName)
        {
            return batchName.Length >= 7 && batchName.Substring(0, 7).Equals(BroadcastCriteria.OfflineRecord, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsBatchPending(string batchName)
        {
            return batchName.Length >= 7 && batchName.ToUpper().Contains(BroadcastCriteria.Pending);
        }

        public static bool IsBatchDone(string batchName)
        {
            return batchName.Length >= 4 && batchName.ToUpper().Contains(BroadcastCriteria.Done);
        }

        public static bool IsBatchInError(string batchName)
        {
            return batchName.Length >= 5 && batchName.ToUpper().Contains(BroadcastCriteria.Error);
        }
    }
}
