using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetEquipmentQuery : IQuery<IList<KeyValue>>
    {
        private readonly IMastersQueryable _db;
        public GetEquipmentQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<KeyValue> ExecuteQuery()
        {
            using (_db)
            {
                return _db.MiscParams.Where(x => x.parmcat.Equals("AIREQUIP", StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => new KeyValue { Key = x.parmcode, Value = x.parmdesc }).ToList();
            }
        }
    }
}
