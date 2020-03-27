using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class OpenClientQuery<T> : IQuery<List<T>>//, IDisposable
    {
        private IClientQueryable _db;
        public string Sql { get; set; }
        public object[] Parameters { get; set; }
        bool disposed;

        public OpenClientQuery(IClientQueryable db, string sql, object[] parameters)
        {
            _db = db;
            Sql = sql;
            Parameters = parameters;
        }
        //~OpenClientQuery()
        //{
        //    Dispose();
        //}

        public List<T> ExecuteQuery()
        {
            using (_db)
            {
                var results = _db.Database.SqlQuery<T>(Sql, Parameters).ToList();
                return results;
            }
        }

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!disposed)
        //    {
        //        if (disposing)
        //        {
        //            //dispose managed resources
        //            if (_db != null) _db.Dispose();
        //        }
        //    }
        //    //dispose unmanaged resources
        //    disposed = true;
        //}

        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}
    }
}
