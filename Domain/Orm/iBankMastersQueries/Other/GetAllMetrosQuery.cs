using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetAllMetrosQuery : IQuery<IList<MetroInformation>>
    {
        private readonly IMastersQueryable _db;

        public GetAllMetrosQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<MetroInformation> ExecuteQuery()
        {
            using(_db)
            {
                return _db.Metro.Select(s => new MetroInformation()
                                                 {
                                                     RecordNo = s.RecordNo,
                                                     MetroCode = s.metrocode,
                                                     MetroCity = s.metrocity,
                                                     MetroState = s.metrostate,
                                                     CountryCode = s.CtryCode
                                                 }).ToList();
            }
        }
    }
}
