using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public abstract class BaseiBankClientQueryable<TResult> : IQuery<TResult>
    {
        protected IClientQueryable _db;
        public abstract TResult ExecuteQuery();
    }
}
