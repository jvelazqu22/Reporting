using System;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetMiscParamQuery : IQuery<KeyValue>
    {
        private readonly string _param;
        private readonly string _langCode;
        private readonly IMastersQueryable _db;

        public GetMiscParamQuery(IMastersQueryable db, string param,  string langCode)
        {
            _db = db;
            _param = param;
            _langCode = langCode;
        }

        public KeyValue ExecuteQuery()
        {
            using (_db)
            {
                
                var parm = _db.MiscParams.FirstOrDefault(x => x.parmcat.Equals(_param, StringComparison.InvariantCultureIgnoreCase) && x.LangCode.Equals(_langCode));
                if (parm == null) return null;
                return new KeyValue {Key = parm.parmcat, Value = parm.parmcode};

            }
        }
    }
    
}
