using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankAdministrationQueries
{
    public class GetBroadcastServerConfigurationQuery : IQuery<IList<BroadcastServerConfiguration>>
    {
        private readonly IAdministrationQueryable _db;

        public GetBroadcastServerConfigurationQuery(IAdministrationQueryable db)
        {
            _db = db;
        }

        public IList<BroadcastServerConfiguration> ExecuteQuery()
        {
            using (_db)
            {
                return (from bsf in _db.BroadcastServerFunction
                        join bs in _db.BroadcastServers on bsf.id equals bs.server_function_id
                        select new BroadcastServerConfiguration
                        {
                            ServerNumber = bs.server_number,
                            Function = (BroadcastServerFunction) bsf.id
                        }).ToList();
            }
        }
    }
}
