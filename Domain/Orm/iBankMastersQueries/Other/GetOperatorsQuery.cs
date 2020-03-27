using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries
{
    public class GetOperatorsQuery : IQuery<IList<KeyValue>>
    {
        private readonly IMastersQueryable _db;

        public GetOperatorsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<KeyValue> ExecuteQuery()
        {
            using (_db)
            {                
                //operators - parmcat: DD_ADVANCEOPERS, Key: parmcode, Value: parmdesc
                return _db.MiscParams.Where(x => x.parmcat.Equals("DD_ADVANCEOPERS", StringComparison.InvariantCultureIgnoreCase))
                .Select(x => new KeyValue { Key = x.parmcode.Trim(), Value = x.parmdesc.Trim(), LangCode = x.LangCode.Trim() }).ToList();
            }
        }
    }
}
