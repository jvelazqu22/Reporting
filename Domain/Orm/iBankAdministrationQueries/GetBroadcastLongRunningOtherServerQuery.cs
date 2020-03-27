using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankAdministrationQueries
{
    public class GetBroadcastLongRunningOtherServerQuery : IQuery<List<int>>
    {
        private readonly IAdministrationQueryable _db;
        private int _server_number;

        public GetBroadcastLongRunningOtherServerQuery(IAdministrationQueryable db, int serverNumber)
        {
            _db = db;
            _server_number = serverNumber;
        }

        public List<int> ExecuteQuery()
        {

            using (_db)
            {
                //check if the server_number passed in is a long-running server
                var list = _db.BroadcastServers.Where(x => x.server_number == _server_number && (BroadcastServerFunction)x.server_function_id == BroadcastServerFunction.LongRunning);

                if (!list.Any()) return new List<int>();
                return _db.BroadcastServers.Where(x=> x.server_number != _server_number && (BroadcastServerFunction)x.server_function_id == BroadcastServerFunction.LongRunning)
                            .Select(x=>x.server_number).ToList();
                
            }
        }
    
    }
}
