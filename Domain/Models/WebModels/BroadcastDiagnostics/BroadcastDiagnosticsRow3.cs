using Domain.Constants;

namespace Domain.Models.WebModels.BroadcastDiagnostics
{
    public class BroadcastDiagnosticsRow3
    {
        public int NumberOfServerHot27RunningBrodcastInTheQueue { get; set; } = 0;
        public int NumberOfServerStage200RunningBrodcastInTheQueue { get; set; } = 0;
        public int NumberOfHighAlertAgencies { get; set; } = 0;
        public int NumberOfLongRunningAgencies { get; set; } = 0;
        public int Empty { get; set; } = 0;

        public string GetNumberOfServerHot27RunningBrodcastInTheQueueTileColor()
        {
            return Enums.TileColors.light.ToString();
        }

        public string GetNumberOfServerHot27RunningBrodcastInTheQueueTextColor()
        {
            return GetNumberOfServerHot27RunningBrodcastInTheQueueTileColor() == Enums.TileColors.light.ToString()
                ? string.Empty
                : "text-white";
        }

        public string GetNumberOfServerStage200RunningBrodcastInTheQueueTileColor()
        {
            return Enums.TileColors.light.ToString();
        }

        public string GetNumberOfServerStage200RunningBrodcastInTheQueueTextColor()
        {
            return GetNumberOfServerStage200RunningBrodcastInTheQueueTileColor() == Enums.TileColors.light.ToString()
                ? string.Empty
                : "text-white";
        }

        public string GetNumberOfHighAlertAgenciesTileColor()
        {
            return NumberOfHighAlertAgencies > 0
                ? Enums.TileColors.danger.ToString()
                : Enums.TileColors.success.ToString();
        }

        public string GetNumberOfHighAlertAgenciesTextColor()
        {
            return GetNumberOfHighAlertAgenciesTileColor() == Enums.TileColors.light.ToString()
                ? string.Empty
                : "text-white";
        }

        public string GetNumberOfLongRunningAgenciesTileColor()
        {
            return NumberOfLongRunningAgencies > 0
                ? Enums.TileColors.warning.ToString()
                : Enums.TileColors.success.ToString();
        }

        public string GetNumberOfLongRunningAgenciesTextColor()
        {
            return GetNumberOfLongRunningAgenciesTileColor() == Enums.TileColors.light.ToString()
                ? string.Empty
                : "text-white";
        }

        public string GetEmptyTileColor()
        {
            return Enums.TileColors.light.ToString();
        }

        public string GetEmptyTextColor()
        {
            return GetEmptyTileColor() == Enums.TileColors.light.ToString()
                ? string.Empty
                : "text-white";
        }

    }
}
