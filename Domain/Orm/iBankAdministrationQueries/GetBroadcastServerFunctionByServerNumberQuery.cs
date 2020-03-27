using Domain.Helper;

using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankAdministrationQueries
{
    public class GetBroadcastServerFunctionByServerNumberQuery : IQuery<BroadcastServerFunction>
    {
        private int ServerNumber { get; }

        private readonly IAdministrationQueryable _db;

        public GetBroadcastServerFunctionByServerNumberQuery(IAdministrationQueryable db, int serverNumber)
        {
            _db = db;
            ServerNumber = serverNumber;
        }

        public BroadcastServerFunction ExecuteQuery()
        {
            using (_db)
            {
                var server = _db.BroadcastServers.FirstOrDefault(x => x.server_number == ServerNumber);

                return server == null ? BroadcastServerFunction.Primary : (BroadcastServerFunction)server.server_function_id;
            }
        }
    }
}
