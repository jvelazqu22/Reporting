using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.Interfaces
{
    public abstract class AbstractRemoveCommand<TEntity> : ICommand where TEntity : class
    {
        public ICommandDb Db { get; set; }
        public IList<TEntity> RecordsToRemove { get; set; }

        protected AbstractRemoveCommand(ICommandDb db, TEntity recToRemove)
        {
            Db = db;
            RecordsToRemove = new List<TEntity> { recToRemove };
        }

        protected AbstractRemoveCommand(ICommandDb db, IList<TEntity> recsToRemove)
        {
            Db = db;
            RecordsToRemove = recsToRemove;
        }

        public void ExecuteCommand()
        {
            Db.Remove(RecordsToRemove);
        }
    }
}
