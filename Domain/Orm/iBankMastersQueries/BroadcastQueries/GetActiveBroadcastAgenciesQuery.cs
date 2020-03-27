using System.Collections.Generic;
using System.Linq;

using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetActiveBroadcastAgenciesQuery : IQuery<IList<AgencyInformation>>
    {
        private readonly IMastersQueryable _db;

        public GetActiveBroadcastAgenciesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<AgencyInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.MstrAgcy.Where(s => s.bcactive.HasValue && s.bcactive.Value)
                                    .Select(s => new AgencyInformation
                                    {
                                        Agency = s.agency.Trim().ToUpper(),
                                        DatabaseName = s.databasename.Trim().ToLower(),
                                        Active = s.active,
                                        BcActive = s.bcactive.HasValue && s.bcactive.Value,
                                        TimeZoneOffset = s.tzoffset ?? 0
                                    }).ToList();
            }
        }
    }
}
