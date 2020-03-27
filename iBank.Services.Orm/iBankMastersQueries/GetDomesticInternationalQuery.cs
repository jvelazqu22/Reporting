using System;
using System.Collections.Generic;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetDomesticInternationalQuery : BaseiBankMastersQuery<IList<KeyValue>>
    {
        private string _langCode;

        public GetDomesticInternationalQuery(IMastersQueryable db, string langCode)
        {
            _db = db;
            _langCode = langCode;
        }

        public override IList<KeyValue> ExecuteQuery()
        {
            using(_db)
            {
                return _db.MiscParams.Where(x => x.parmcat.Equals("DD_DOMINTL", StringComparison.InvariantCultureIgnoreCase) && x.LangCode.Equals(_langCode, StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => new KeyValue { Key = x.parmcode.Trim(), Value = x.parmdesc.Trim() }).ToList();
            }
        }
    }
}
