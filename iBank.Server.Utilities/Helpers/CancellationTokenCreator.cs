using System.Configuration;
using System.Threading;

namespace iBank.Server.Utilities.Helpers
{
    public static class CancellationTokenCreator
    {
        public static CancellationToken Create(int msToWaitBeforeCancellation)
        {
            var cancelSrc = new CancellationTokenSource();
            cancelSrc.CancelAfter(msToWaitBeforeCancellation);
            var cancelToken = cancelSrc.Token;
            cancelToken.ThrowIfCancellationRequested();
            return cancelToken;
        }

        public static CancellationToken Create(string configurationSettingKey = "PushOfflineIntervalInMs")
        {
            var cancelSrc = new CancellationTokenSource();
            var msToWaitBeforeCancellation = GetTimeToWaitInMs(configurationSettingKey);
            cancelSrc.CancelAfter(msToWaitBeforeCancellation);
            var cancelToken = cancelSrc.Token;
            cancelToken.ThrowIfCancellationRequested();
            return cancelToken;
        }

        public static int GetTimeToWaitInMs(string configSettingKey)
        {
#if DEBUG
            return int.MaxValue;
#else
            var temp = ConfigurationManager.AppSettings["PushOfflineIntervalInMs"];
            return int.Parse(temp);
#endif
        }
    }
}
