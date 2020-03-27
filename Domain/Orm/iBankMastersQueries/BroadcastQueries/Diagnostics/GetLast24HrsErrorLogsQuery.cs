using System;
using System.Collections.Generic;
using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries.Diagnostics
{
    public class GetLast24HrsErrorLogsQuery : IQuery<IList<reporting_error_log>>
    {
        private readonly IMastersQueryable _db;

        public GetLast24HrsErrorLogsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<reporting_error_log> ExecuteQuery()
        {
            var errors = new List<string>() { "FATAL","ERROR"};
            var broadcastServers = new List<string>() {"20","25","26","21","24","27" };

            using (_db)
            {
                var oneDayAgoDate = DateTime.Now.AddHours(-24);
                var results = _db.ReportingErrorLog
                    .Where(w => w.time_stamp >= oneDayAgoDate)
                    .Where(w => errors.Contains(w.log_level))
                    .Where(w => broadcastServers.Contains((w.server_number)))
                    .OrderByDescending(o => o.time_stamp)
                    .ToList();
                return results;
            }
        }
    }
}
