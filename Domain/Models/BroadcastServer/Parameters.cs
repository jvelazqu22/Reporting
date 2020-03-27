using Domain.Helper;
using Domain.Interfaces.BroadcastServer;
using Domain.Interfaces.Query;

using System.Collections.Concurrent;
using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Models.BroadcastServer
{
    public class Parameters
    {
        public Parameters(IBroadcastQueueRecordRemover broadcastQueueRecordRemover, IBroadcastRecordUpdatesManager batchRecordUpdatesManager)
        {
            BroadcastQueueRecordRemover = broadcastQueueRecordRemover;
            BatchRecordUpdatesManager = batchRecordUpdatesManager;
        }
        public BroadcastServerInformation ServerConfiguration { get; set; }
        public string UnilanguageCode { get; set; }

        public IMasterDataStore MasterDataStore { get; set; }

        public readonly IBroadcastQueueRecordRemover BroadcastQueueRecordRemover;

        public readonly IBroadcastRecordUpdatesManager BatchRecordUpdatesManager;
        public bool IsMaintenanceModeRequested { get; set; }

        public BlockingCollection<bcstque4> BatchesToExecute { get; set; }
        public IDatabaseInfoQuery DatabaseInfoQuery { get; set; } = null;
        public IClientDataStore ClientDataStore { get; set; } = null;
        public IQuery<ibuser> GetUserByUserNumberQuery { get; set; } = null;
        public IUserBroadcastSettings UserBroadcastSettings { get; set; } = null;
        public IBroadcastRecordRetriever BroadcastRecordRetriever { get; set; } = null;
        public IBatchManager BatchManager { get; set; } = null;
        public IQuery<IList<ProcessCaptionInformation>> GetAllActiveBroadcastProcessCaptionsQuery { get; set; } = null;
        public IBroadcastReportProcessor BroadcastReportProcessor { get; set; } = null;
        public IBatchReportRetriever BatchReportRetriever { get; set; } = null;
        public IBatchRunner BatchRunner { get; set; } = null;
    }
}
