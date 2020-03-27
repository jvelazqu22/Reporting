using Domain.Helper;
using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using System.Linq;

namespace iBank.Services.Orm.iBankAdministrationQueries
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

                if (server == null)
                {
                    return BroadcastServerFunction.Primary;
                }
                else
                {
                    var serverFunctionId = server.server_function_id;

                    switch (serverFunctionId)
                    {
                        case 1:
                            return BroadcastServerFunction.Primary;
                        case 2:
                            return BroadcastServerFunction.Offline;
                        case 3:
                            return BroadcastServerFunction.Hot;
                        case 4:
                            return BroadcastServerFunction.AgencyStage;
                        case 5:
                            return BroadcastServerFunction.DatabaseStage;
                        case 6:
                            return BroadcastServerFunction.LongRunning;
                        default:
                            return BroadcastServerFunction.Primary;
                    }
                }
            }
        }
    }
}
