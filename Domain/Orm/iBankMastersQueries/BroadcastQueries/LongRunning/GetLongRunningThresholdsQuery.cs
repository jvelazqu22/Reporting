using System;
using System.Collections.Generic;
using System.Linq;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries.LongRunning
{
    public class GetLongRunningThresholdsQuery : IQuery<Dictionary<string, int>>
    {
        private readonly IMastersQueryable _db;
        
        public GetLongRunningThresholdsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns a case insensitive dictionary
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> ExecuteQuery()
        {
            using (_db)
            {
                return  _db.BroadcastLongRunningThreshold
                          .ToDictionary(key => key.Agency.Trim(), val => val.MonthsInRangeThreshold, StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
