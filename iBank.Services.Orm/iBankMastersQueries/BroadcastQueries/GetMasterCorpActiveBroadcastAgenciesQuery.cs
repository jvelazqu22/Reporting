using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetMasterCorpActiveBroadcastAgenciesQuery : BaseiBankMastersQuery<IList<AgencyInformation>>
    {
        public GetMasterCorpActiveBroadcastAgenciesQuery(IMastersQueryable db)
        {
            _db = db;
        }
        public override IList<AgencyInformation> ExecuteQuery()
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
