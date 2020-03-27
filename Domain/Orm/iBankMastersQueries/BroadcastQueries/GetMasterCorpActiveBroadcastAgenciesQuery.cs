using System.Collections.Generic;
using System.Linq;

using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetMasterCorpActiveBroadcastAgenciesQuery : IQuery<IList<AgencyInformation>>
    {
        private readonly IMastersQueryable _db;

        public GetMasterCorpActiveBroadcastAgenciesQuery(IMastersQueryable db)
        {
            _db = db;
        }
        public IList<AgencyInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.MstrCorpAccts.Where(s => s.bcactive)
                    .Select(s => new AgencyInformation
                                     {
                                         Agency = s.CorpAcct.Trim().ToUpper(),
                                         DatabaseName = s.databasename.Trim().ToLower(),
                                         Active = s.active,
                                         BcActive = s.bcactive,
                                         TimeZoneOffset = s.tzoffset ?? 0
                                     }).ToList();
            }
        }
    }
}
