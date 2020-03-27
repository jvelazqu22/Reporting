using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetMultipleBroadcastsAllowedByServerNumberQuery : BaseiBankMastersQuery<IList<MultiAccess>>
    {
        public int ServerNumber { get; set; }

        public GetMultipleBroadcastsAllowedByServerNumberQuery(IMastersQueryable db, int serverNumber)
        {
            _db = db;
            ServerNumber = serverNumber;
        }

        public override IList<MultiAccess> ExecuteQuery()
        {
            using (_db)
            {
                return _db.BcstAllowMultiples.Where(s => s.svrnumber == ServerNumber)
                    .Select(s => new MultiAccess
                                     {
                                         DBName = s.dbname.Trim().ToUpper(),
                                         Agency = s.agency.Trim().ToUpper()
                                     }).ToList();
            }
        }
    }
}
