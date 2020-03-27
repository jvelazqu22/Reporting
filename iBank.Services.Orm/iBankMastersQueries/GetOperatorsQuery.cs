using System;
using System.Collections.Generic;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetOperatorsQuery : BaseiBankMastersQuery<IList<KeyValue>>
    {
        public GetOperatorsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public override IList<KeyValue> ExecuteQuery()
        {
            using (_db)
            {
                return _db.MiscParams.Where(x => x.parmcat.Equals("DD_DOMINTL", StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => new KeyValue { Key = x.parmcat, Value = x.parmcode }).ToList();
            }
        }
    }
}
