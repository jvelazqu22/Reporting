using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientCommands.OverdueMonitor
{
    public class UpdateOverdueBroadcastsCommand : AbstractUpdateCommand<overdue_broadcasts>
    {
        public UpdateOverdueBroadcastsCommand(ICommandDb db, overdue_broadcasts recToUpdate) : base(db, recToUpdate)
        {
        }

        public UpdateOverdueBroadcastsCommand(ICommandDb db, IList<overdue_broadcasts> recsToUpdate) : base(db, recsToUpdate)
        {
        }
    }
}
