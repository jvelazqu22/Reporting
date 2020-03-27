using System;
using System.Linq;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries.BroadcastHistoryQueries
{
    public class GetBroadcastHistoryMissingCreatedOfRunQuery : IQuery<BroadcastHistory>
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType); private readonly IMastersQueryable _db;
        private readonly bcstque4 _bcstque4;

        public GetBroadcastHistoryMissingCreatedOfRunQuery(IMastersQueryable db, bcstque4 recToUpdate)
        {
            _db = db;
            _bcstque4 = recToUpdate;
        }

        public BroadcastHistory ExecuteQuery()
        {
            try
            {
                using (_db)
                {
                    return _db.BroadcastHistory
                        .Where(x => x.batchname.Equals(_bcstque4.batchname))
                        .Where(x => x.batchnum == _bcstque4.batchnum)
                        .Where(x => x.agency.Equals(_bcstque4.agency, StringComparison.OrdinalIgnoreCase))
                        .Where(x => !x.start_of_run.HasValue)
                        .OrderByDescending(o => o.created_on)
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                if (_db == null) LOG.Error($"_db is null i n GetBroadcastHistoryMissingCreatedOfRunQuery");

                throw ex;
            }
        }
    }
}
