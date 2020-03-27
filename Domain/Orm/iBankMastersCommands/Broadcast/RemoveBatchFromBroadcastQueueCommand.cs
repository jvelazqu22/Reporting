using System.Collections.Generic;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands.Broadcast
{
    public class RemoveBatchFromBroadcastQueueCommand : AbstractRemoveLetClientWinCommand<bcstque4>
    {
        public RemoveBatchFromBroadcastQueueCommand(ICommandDb db, bcstque4 recToRemove) : base(db, recToRemove)
        {
        }

        public RemoveBatchFromBroadcastQueueCommand(ICommandDb db, IList<bcstque4> recsToRemove) : base(db, recsToRemove)
        {
        }
    }
}
