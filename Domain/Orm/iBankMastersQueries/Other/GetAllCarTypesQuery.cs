using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetAllCarTypesQuery : IQuery<IList<CarTypeInfo>>
    {
        private readonly IMastersQueryable _db;

        public GetAllCarTypesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<CarTypeInfo> ExecuteQuery()
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
