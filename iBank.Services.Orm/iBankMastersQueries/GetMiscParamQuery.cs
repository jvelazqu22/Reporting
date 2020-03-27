using System;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetMiscParamQuery : BaseiBankMastersQuery<KeyValue>
    {
        private readonly string _param;
        private readonly string _langCode;
        public GetMiscParamQuery(IMastersQueryable db, string param,  string langCode)
        {
            _db = db;
            _param = param;
            _langCode = langCode;
        }

        public override KeyValue ExecuteQuery()
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
