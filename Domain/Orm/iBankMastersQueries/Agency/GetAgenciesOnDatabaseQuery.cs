using System.Collections.Generic;
using System.Linq;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Agency
{
    public class GetAgenciesOnDatabaseQuery : IQuery<IList<string>>
    {
        private readonly IMastersQueryable _db;

        private readonly IList<string> _databaseNames;

        public GetAgenciesOnDatabaseQuery(IMastersQueryable db, IList<string> databaseNames)
        {
            _db = db;
            _databaseNames = databaseNames;
        }

        public IList<string> ExecuteQuery()
        {
            using (_db)
            {
                return _db.MstrAgcy.Where(x => _databaseNames.Contains(x.databasename.Trim()))
                                    .Select(x => x.agency.Trim())
                                    .ToList();
            }
        }
    }
}
