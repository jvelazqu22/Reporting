using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetAllWorldRegionsQuery : IQuery<IList<KeyValue>>
    {
        private readonly IMastersQueryable _db;

        public GetAllWorldRegionsQuery(IMastersQueryable db)
        {
            _db = db;
        }
        public IList<KeyValue> ExecuteQuery()
        {
            using (_db)
            {
                return _db.WorldRegions.Select(x => new KeyValue
                                                        {
                                                            Key = x.RegionCode.Trim(),
                                                            Value = x.RegionName.Trim(),
                                                            LangCode = x.LangCode.Trim()
                                                        }).ToList();
            }
        }
    }
}
