using Domain.Extensions;
using iBank.Entities.MasterEntities;
using System.Linq;
using System;
using iBank.Repository.SQL.Interfaces;
using Domain.Helper;
using Domain.Services;
using iBank.Repository.SQL.Repository;
using Domain.Constants;
using System.Collections.Generic;

namespace iBank.BroadcastServer.QueueManager.BuildQueue
{
    public static class BroadcastTypeIdentifier
    {

        public static string GetBroadcastType(bcstque4 bcst, IList<broadcast_stage_agencies> loggingAgenciesList, Dictionary<string, int> longRunningThresholds, int defaultThreshold)
        {

            if (bcst.IsStageType()) return BroadcastTypes.Stage;

            if (bcst.IsHotType(loggingAgenciesList)) return BroadcastTypes.Hot;

            if (bcst.IsLoggingType(loggingAgenciesList)) return BroadcastTypes.Logging;

            if (bcst.IsLongrunningType(loggingAgenciesList, longRunningThresholds, defaultThreshold)) return BroadcastTypes.LongRunning;

            if (bcst.IsOfflineType(loggingAgenciesList)) return BroadcastTypes.Offline;

            return BroadcastTypes.Primary;
        }

    }
}
