using Domain.Constants;

namespace Domain.Models.WebModels.BroadcastDiagnostics
{
    public class BroadcastDiagnosticsRow1
    {
        public int TotalNumberOfBrodcastInTheQueue { get; set; } = 0;
        public int NumberOfPendingBrodcastInTheQueue { get; set; } = 0;
        public int NumberOfRunningBrodcastInTheQueue { get; set; } = 0;
        public int NumberOfServerPrimary20RunningBrodcastInTheQueue { get; set; } = 0;

        public string TotalNumberOfBrodcastInTheQueueFooter { get; set; } = "";
        public string NumberOfPendingBrodcastInTheQueueFooter { get; set; } = "";
        public string NumberOfRunningBrodcastInTheQueueFooter { get; set; } = "";
        public string NumberOfServerPrimary20RunningBrodcastInTheQueueFooter { get; set; } = "";

        public string GetTotalNumberOfBrodcastInTheQueueTileColor()
        {
            return Enums.TileColors.light.ToString();
        }

        public string GetTotalNumberOfBrodcastInTheQueueTextColor()
        {
            return GetTotalNumberOfBrodcastInTheQueueTileColor() == Enums.TileColors.light.ToString()
                ? string.Empty
                : "text-white";
        }

        public string GetNumberOfPendingBrodcastInTheQueueTileColor()
        {
            return Enums.TileColors.light.ToString();
        }

        public string GetNumberOfPendingBrodcastInTheQueueTextColor()
        {
            return GetNumberOfPendingBrodcastInTheQueueTileColor() == Enums.TileColors.light.ToString()
                ? string.Empty
                : "text-white";
        }

        public string GetNumberOfRunningBrodcastInTheQueueTileColor()
        {
            return Enums.TileColors.light.ToString();
        }

        public string GetNumberOfRunningBrodcastInTheQueueTextColor()
        {
            return GetNumberOfRunningBrodcastInTheQueueTileColor() == Enums.TileColors.light.ToString()
                ? string.Empty
                : "text-white";

        }

        public string GetNumberOfServerPrimary20RunningBrodcastInTheQueueTileColor()
        {
            return Enums.TileColors.light.ToString();
        }

        public string GetNumberOfServerPrimary20RunningBrodcastInTheQueueTextColor()
        {
            return GetNumberOfServerPrimary20RunningBrodcastInTheQueueTileColor() == Enums.TileColors.light.ToString()
                ? string.Empty
                : "text-white";

        }


#if DEBUG || Keystone

#else
    #if Production

    #endif
    #if ProductionGSA
      
    #endif

#endif
    }
}
