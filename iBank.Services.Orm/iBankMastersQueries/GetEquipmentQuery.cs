using System;
using System.Collections.Generic;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetEquipmentQuery : BaseiBankMastersQuery<IList<KeyValue>>
    {
        public GetEquipmentQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public override IList<KeyValue> ExecuteQuery()
        {
            using (_db)
            {
                return _db.MiscParams.Where(x => x.parmcat.Equals("AIREQUIP", StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => new KeyValue { Key = x.parmcode, Value = x.parmdesc }).ToList();
            }
        }
    }
}
