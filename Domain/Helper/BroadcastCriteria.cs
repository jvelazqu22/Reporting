namespace Domain.Helper
{
    public static class BroadcastCriteria
    {
        public static readonly string HoldOnRecord = "H";

        public static readonly string TravetRecord = "SYSTR:";

        public static readonly string OfflineRecord = "sysDR:[";

        public static readonly string Pending = "PENDING";

        public static readonly string Running = "RUNNING";

        public static readonly string Done = "DONE";

        public static readonly string Error = "ERROR";

        public static readonly string EffectsOutputDest = "3";
    }
}
