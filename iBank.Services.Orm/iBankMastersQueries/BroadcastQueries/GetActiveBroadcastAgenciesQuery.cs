using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetActiveBroadcastAgenciesQuery : BaseiBankMastersQuery<IList<AgencyInformation>>
    {
        public GetActiveBroadcastAgenciesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public override IList<AgencyInformation> ExecuteQuery()
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
