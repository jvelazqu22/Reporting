using Domain.Helper;
using Domain.Models.BroadcastServer;
using System.Collections.Generic;
using Domain.Services;

namespace Domain.Interfaces.BroadcastServer
{
    public interface IBatchRunner
    {
        void ExecuteBatch(ICacheService cache, IList<BroadcastReportInformation> allReportsInBatch, IRecordTimingDetails broadcastRecTiming, BroadcastServerInformation serverConfig, bool runSpecial, bool isOfflineBatch);
    }
}
