using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetDomesticInternationalQuery : IQuery<IList<KeyValue>>
    {
        private string _langCode;
        private readonly IMastersQueryable _db;

        public GetDomesticInternationalQuery(IMastersQueryable db, string langCode)
        {
            _db = db;
            _langCode = langCode;
        }

        public IList<KeyValue> ExecuteQuery()
        {
            using(_db)
            {
                return _db.MiscParams.Where(x => x.parmcat.Equals("DD_DOMINTL", StringComparison.InvariantCultureIgnoreCase) 
                                                && x.LangCode.Equals(_langCode, StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => new KeyValue { Key = x.parmcode.Trim(), Value = x.parmdesc.Trim() }).ToList();
            }
        }
    }
}
