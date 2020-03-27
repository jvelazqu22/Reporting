using Domain.Interfaces;
using iBank.Services.Orm.Databases;

namespace iBank.Services.Orm.CISMastersQueries
{
    public abstract class BaseCISMastersQuery<TResult> : IQuery<TResult>
    {
        protected CisMastersQueryable _db;
        public abstract TResult ExecuteQuery();
    }
}
