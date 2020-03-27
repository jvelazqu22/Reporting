using System.Collections.Generic;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetAllCarTypesQuery : BaseiBankMastersQuery<IList<CarTypeInfo>>
    {
        public GetAllCarTypesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public override IList<CarTypeInfo> ExecuteQuery()
        {
            using (_db)
            {
                return _db.CarTypes.OrderBy(x => x.LangCode).ThenBy(x => x.cartype).Select(s => new CarTypeInfo
                                                                                                    {
                                                                                                        CarType = s.cartype.Trim(),
                                                                                                        Description = s.ctypedesc.Trim(),
                                                                                                        LanguageCode = s.LangCode.Trim()
                                                                                                    }).ToList();
            }
        }
    }
}
