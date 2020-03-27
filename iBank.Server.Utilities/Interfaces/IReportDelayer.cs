namespace iBank.Server.Utilities.Interfaces
{
    public interface IReportDelayer
    {
        void PushReportOffline(string serverNumber, string offlineMessage = null);
    }
}