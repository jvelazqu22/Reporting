using System.Collections.Generic;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands.Broadcast
{
    public class RemoveBroadcastHistoryCommand : AbstractRemoveLetClientWinCommand<BroadcastHistory>
    {
        public RemoveBroadcastHistoryCommand(ICommandDb db, BroadcastHistory recToRemove) : base(db, recToRemove)
        {
        }

        public RemoveBroadcastHistoryCommand(ICommandDb db, IList<BroadcastHistory> recsToRemove) : base(db, recsToRemove)
        {
        }
    }
}
