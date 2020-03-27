using Domain.Helper;
using Domain.Models.BroadcastServer;

using System.Collections.Generic;
using Domain.Services;
using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;

namespace Domain.Interfaces.BroadcastServer
{
    public interface IBroadcastReportProcessor
    {
        bool RunAllReports(ICacheService cache, IList<BroadcastReportInformation> allReportsInBatch, IRecordTimingDetails broadcastRecTiming,
                            BroadcastServerInformation serverConfiguration, bool isOfflineReport, ibuser user, bool runSpecial, bcstque4 queueRecord);
    }
}
