using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public abstract class BaseiBankMastersQuery<TResult> : IQuery<TResult>
    {
        protected IMastersQueryable _db;

        public abstract TResult ExecuteQuery();
    }
}
