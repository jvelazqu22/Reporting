using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetMiscParamListQuery : BaseiBankMastersQuery<IList<KeyValue>>
    {
        private readonly string _param;
        private readonly string _langCode;
        public GetMiscParamListQuery(IMastersQueryable db, string param, string langCode)
        {
            _db = db;
            _param = param;
            _langCode = langCode;
        }

        public override IList<KeyValue> ExecuteQuery()
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
