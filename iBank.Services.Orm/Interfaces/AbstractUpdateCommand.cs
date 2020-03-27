using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.Interfaces
{
    public abstract class AbstractUpdateCommand<TEntity> : ICommand where TEntity : class
    {
        public ICommandDb Db { get; set; }

        public IList<TEntity> RecordsToUpdate { get; set; }

        protected AbstractUpdateCommand(ICommandDb db, TEntity recToUpdate)
        {
            Db = db;
            RecordsToUpdate = new List<TEntity> { recToUpdate };
        }

        protected AbstractUpdateCommand(ICommandDb db, IList<TEntity> recsToUpdate)
        {
            Db = db;
            RecordsToUpdate = recsToUpdate;
        }
        
        public void ExecuteCommand()
        {
            Db.Update(RecordsToUpdate);
        }
    }
}
