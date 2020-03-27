namespace Domain.Models.WebModels.BroadcastDiagnostics
{
    public class MainDiagnostics
    {
        // BroadcastDiagnosticsRow1
        public int TotalNumberOfBrodcastInTheQueue { get; set; } = 0;
        public int NumberOfPendingBrodcastInTheQueue { get; set; } = 0;
        public int NumberOfRunningBrodcastInTheQueue { get; set; } = 0;
        public int NumberOfServerPrimary20RunningBrodcastInTheQueue { get; set; } = 0;

        // BroadcastDiagnosticsRow2
        public int NumberOfServerPrimary21RunningBrodcastInTheQueue { get; set; } = 0;
        public int NumberOfServerOffline24RunningBrodcastInTheQueue { get; set; } = 0;
        public int NumberOfServerOffline25RunningBrodcastInTheQueue { get; set; } = 0;
        public int NumberOfServerHot26RunningBrodcastInTheQueue { get; set; } = 0;

        // BroadcastDiagnosticsRow3
        public int NumberOfServerHot27RunningBrodcastInTheQueue { get; set; } = 0;
        public int NumberOfServerStage200RunningBrodcastInTheQueue { get; set; } = 0;
        public int NumberOfHighAlertAgencies { get; set; } = 0;
        public int NumberOfLongRunningAgencies { get; set; } = 0;

    }
}
