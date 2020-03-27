using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetAllWorldRegionsQuery : BaseiBankMastersQuery<IList<KeyValue>>
    {
        public GetAllWorldRegionsQuery(IMastersQueryable db)
        {
            _db = db;
        }
        public override IList<KeyValue> ExecuteQuery()
        {
            using (_db)
            {
                return _db.WorldRegions.Select(x => new KeyValue
                                                        {
                                                            Key = x.RegionCode,
                                                            Value = x.RegionName
                                                        }).ToList();
            }
        }
    }
}
