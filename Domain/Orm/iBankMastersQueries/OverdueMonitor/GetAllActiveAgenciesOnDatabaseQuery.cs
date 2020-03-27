using System;
using System.Collections.Generic;
using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.OverdueMonitor
{
    public class GetAllActiveAgenciesOnDatabaseQuery : IQuery<IList<string>>
    {
        private readonly IMastersQueryable _db;

        private readonly string _databaseName;

        public GetAllActiveAgenciesOnDatabaseQuery(IMastersQueryable db, string databaseName)
        {
            _db = db;
            _databaseName = databaseName.Trim();
        }

        public IList<string> ExecuteQuery()
        {
            using (_db)
            {
                return _db.MstrAgcy.Where(x => !string.IsNullOrEmpty(x.databasename)
                                               && x.databasename.Trim().Equals(_databaseName, StringComparison.OrdinalIgnoreCase)
                                               && x.active)
                                    .Select(x => x.agency).ToList();
            }
        }
    }
}
