using System;
using System.Collections.Generic;
using Domain.Helper;
using Domain.Services;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.Interfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries.BroadcastRetrieval
{
    public class BroadcastRetrievalFactory : IFactory<List<bcstque4>>
    {
        private readonly BroadcastServerFunction _function;
        private IMasterDataStore _store;
        private ICacheService _cache;

        public BroadcastRetrievalFactory(BroadcastServerFunction function, IMasterDataStore store, ICacheService cache)
        {
            _function = function;
            _store = store;
            _cache = cache;
        }

        public List<bcstque4> Build()
        {
            using (var db = _store.MastersQueryDb)
            {
                IBroadcastRetrieval retriever;
                switch (_function)
                {
                    case BroadcastServerFunction.Primary:
                        retriever = new PrimaryServerBroadcastRetrieval(_store, _cache, db.BroadcastStageAgencies);
                        break;
                    case BroadcastServerFunction.Offline:
                        retriever = new OfflineServerBroadcastRetrieval(_store, _cache, db.BroadcastStageAgencies);
                        break;
                    case BroadcastServerFunction.Hot:
                        retriever = new HotServerBroadcastRetrieval(db.BroadcastStageAgencies);
                        break;
                    case BroadcastServerFunction.Stage:
                        retriever = new StageServerBroadcastRetrieval();
                        break;
                    case BroadcastServerFunction.Logging:
                        retriever = new LoggingServerBroadcastRetrieval(db.BroadcastStageAgencies);
                        break;
                    case BroadcastServerFunction.LongRunning:
                        retriever = new LongRunningBroadcastRetrieval(_store, _cache, db.BroadcastStageAgencies);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Broadcast Server Function {_function} not handled.");
                }

                return retriever.GetBroadcasts(db.BcstQue4);
            }
        }
    }
}
