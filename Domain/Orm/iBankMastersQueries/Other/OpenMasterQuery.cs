using System.Collections.Generic;
using System.Linq;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class OpenMasterQuery<T> : IQuery<List<T>>
    {
        private IMastersQueryable _db;
        public string Sql { get; set; }
        public object[] Parameters { get; set; }

        public OpenMasterQuery(IMastersQueryable db, string sql, object[] parameters)
        {
            _db = db;
            Sql = sql;
            Parameters = parameters;
        }

        public List<T> ExecuteQuery()
        {
            using(_db)
            {
                return _db.Database.SqlQuery<T>(Sql, Parameters).ToList();
            }
        }
    }
}
