using System;
using System.Linq;
using Domain.Interfaces.Query;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetDatabaseInfoByDatabaseNameQuery : IDatabaseInfoQuery
    {
        private IMastersQueryable _db;

        public string DatabaseName { get; set; }
        public bool HasDbBeenDisposed { get; set; } = false;

        public GetDatabaseInfoByDatabaseNameQuery(IMastersQueryable db, string databaseName)
        {
            _db = db;
            DatabaseName = databaseName;
        }

        public iBankDatabases ExecuteQuery()
        {
            using(_db)
            {
                HasDbBeenDisposed = true;
                return _db.iBankDatabases.FirstOrDefault(x => x.databasename.Trim().Equals(DatabaseName, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
