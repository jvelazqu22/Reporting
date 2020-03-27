using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iBank.Services.Orm.Databases.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.Interfaces
{
    public abstract class AbstractRemoveLetClientWinCommand<TEntity> : ICommand where TEntity : class
    {
        public ICommandDb Db { get; set; }
        public IList<TEntity> RecordsToRemove { get; set; }

        protected AbstractRemoveLetClientWinCommand(ICommandDb db, TEntity recToRemove)
        {
            Db = db;
            RecordsToRemove = new List<TEntity> { recToRemove };
        }

        protected AbstractRemoveLetClientWinCommand(ICommandDb db, IList<TEntity> recsToRemove)
        {
            Db = db;
            RecordsToRemove = recsToRemove;
        }

        public void ExecuteCommand()
        {
            Db.RemoveLetClientWin(RecordsToRemove);
        }
    }
}
