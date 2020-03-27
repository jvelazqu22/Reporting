using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetMiscParamListQuery : IQuery<IList<KeyValue>>
    {
        private readonly string _param;
        private readonly string _langCode;
        private readonly IMastersQueryable _db;

        public GetMiscParamListQuery(IMastersQueryable db, string param, string langCode)
        {
            _db = db;
            _param = param;
            _langCode = langCode;
        }

        public IList<KeyValue> ExecuteQuery()
        {
            using (_db)
            {

                return _db.MiscParams.Where(
                    x =>
                        x.parmcat.Equals(_param, StringComparison.InvariantCultureIgnoreCase) &&
                        x.LangCode.Equals(_langCode))
                    .Select(s => new KeyValue {Key = s.parmcode.Trim(), Value = s.parmdesc.Trim()}).ToList();

            }
        }
    }
}
