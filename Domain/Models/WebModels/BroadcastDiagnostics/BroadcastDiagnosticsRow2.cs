using Domain.Constants;

namespace Domain.Models.WebModels.BroadcastDiagnostics
{
    public class BroadcastDiagnosticsRow2
    {
        public int NumberOfServerPrimary21RunningBrodcastInTheQueue { get; set; } = 0;
        public int NumberOfServerOffline24RunningBrodcastInTheQueue { get; set; } = 0;
        public int NumberOfServerOffline25RunningBrodcastInTheQueue { get; set; } = 0;
        public int NumberOfServerHot26RunningBrodcastInTheQueue { get; set; } = 0;

        public string GetTotalNumberOfServerPrimary21RunningBrodcastInTheQueueTileColor()
        {
            return Enums.TileColors.light.ToString();
        }

        public string GetNumberOfServerPrimary21RunningBrodcastInTheQueueTextColor()
        {
            return GetTotalNumberOfServerPrimary21RunningBrodcastInTheQueueTileColor() == Enums.TileColors.light.ToString()
                ? string.Empty
                : "text-white";
        }

        public string GetNumberOfServerOffline24RunningBrodcastInTheQueueTileColor()
        {
            return Enums.TileColors.light.ToString();
        }

        public string GetNumberOfServerOffline24RunningBrodcastInTheQueueTextColor()
        {
            return GetNumberOfServerOffline24RunningBrodcastInTheQueueTileColor() == Enums.TileColors.light.ToString()
                ? string.Empty
                : "text-white";
        }

        public string GetNumberOfServerOffline25RunningBrodcastInTheQueueTileColor()
        {
            return Enums.TileColors.light.ToString();
        }

        public string GetNumberOfServerOffline25RunningBrodcastInTheQueueTextColor()
        {
            return GetNumberOfServerOffline25RunningBrodcastInTheQueueTileColor() == Enums.TileColors.light.ToString()
                ? string.Empty
                : "text-white";

        }

        public string GetNumberOfServerHot26RunningBrodcastInTheQueueTileColor()
        {
            return Enums.TileColors.light.ToString();
        }

        public string GetNumberOfServerHot26RunningBrodcastInTheQueueTextColor()
        {
            return GetNumberOfServerHot26RunningBrodcastInTheQueueTileColor() == Enums.TileColors.light.ToString()
                ? string.Empty
                : "text-white";

        }

    }
}
