using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.Interfaces
{
    public abstract class AbstractAddCommand<TEntity> : ICommand where TEntity : class
    {
        public ICommandDb Db { get; set; }
        public IList<TEntity> RecordsToAdd { get; set; }

        protected AbstractAddCommand(ICommandDb db, TEntity recToAdd)
        {
            Db = db;
            RecordsToAdd = new List<TEntity> { recToAdd };
        }

        protected AbstractAddCommand(ICommandDb db, IList<TEntity> recsToAdd)
        {
            Db = db;
            RecordsToAdd = recsToAdd;
        }

        public void ExecuteCommand()
        {
            Db.Insert(RecordsToAdd);
        }
    }
}
