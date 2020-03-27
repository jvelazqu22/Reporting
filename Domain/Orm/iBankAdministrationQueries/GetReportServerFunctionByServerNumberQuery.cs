using Domain.Helper;

using System;
using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankAdministrationQueries
{
    public class GetReportServerFunctionByServerNumberQuery : IQuery<ReportServerFunction>
    {
        private int _serverNumber;

        private IAdministrationQueryable _db;

        public GetReportServerFunctionByServerNumberQuery(IAdministrationQueryable db, int serverNumber)
        {
            _db = db;
            _serverNumber = serverNumber;
        }

        public ReportServerFunction ExecuteQuery()
        {
            using (_db)
            {
                var server = _db.ReportServers.FirstOrDefault(x => x.server_number == _serverNumber);

                if (server == null) throw new Exception($"Server number [{_serverNumber}] not found in ReportServers table!");
                
                switch (server.server_function_id)
                {
                    case 1:
                        return ReportServerFunction.Primary;
                    case 2:
                        return ReportServerFunction.Stage;
                    default:
                        throw new Exception($"For Server Number [{server.server_number}], Server Function Id [{server.server_function_id}] not found.");
                }
            }
        }
    }
}
