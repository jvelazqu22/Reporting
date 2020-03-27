using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;
using System;
using System.Linq;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetDatabaseInfoByDatabaseNameQuery : IQuery<iBankDatabases>
    {
        private IMastersQueryable _db;

        public string DatabaseName { get; set; }

        public GetDatabaseInfoByDatabaseNameQuery(IMastersQueryable db, string databaseName)
        {
            _db = db;
            DatabaseName = databaseName;
        }

        public iBankDatabases ExecuteQuery()
        {
            using(_db)
            {
                return _db.iBankDatabases.FirstOrDefault(x => x.databasename.Trim().Equals(DatabaseName, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
