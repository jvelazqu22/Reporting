using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Extensions;
using Domain.Helper;
using Domain.Orm.iBankMastersQueries.BroadcastQueries.BroadcastRetrieval;
using Domain.Services;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetPendingBroadcastQuery : IQuery<IList<bcstque4>>
    {
        private string UnilanguageCode { get; }
        private BroadcastServerFunction Function { get; }
        private ICacheService _cache;

        private readonly IMasterDataStore _store;

        public GetPendingBroadcastQuery(IMasterDataStore store, string unilanguageCode, BroadcastServerFunction function, 
            ICacheService cache)
        {
            _store = store;
            UnilanguageCode = unilanguageCode;
            Function = function;
            _cache = cache;
        }

        public IList<bcstque4> ExecuteQuery()
        {
            var factory = new BroadcastRetrievalFactory(Function, _store, _cache);
            var broadcastsToProcess = factory.Build();

            return string.IsNullOrEmpty(UnilanguageCode)
                ? broadcastsToProcess.Where(x => string.IsNullOrEmpty(x.unilangcode.Trim())).ToList()
                : broadcastsToProcess.Where(x => x.unilangcode.Trim().Equals(UnilanguageCode.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
