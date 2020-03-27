using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Orm.iBankMastersQueries
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
