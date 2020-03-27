using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientCommands.OverdueMonitor
{
    public class InsertOverdueBroadcastsCommand : AbstractAddCommand<overdue_broadcasts>
    {
        public InsertOverdueBroadcastsCommand(ICommandDb db, overdue_broadcasts recToAdd) : base(db, recToAdd)
        {
        }

        public InsertOverdueBroadcastsCommand(ICommandDb db, IList<overdue_broadcasts> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
