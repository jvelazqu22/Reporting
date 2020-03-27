using System.Collections.Generic;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands.Broadcast
{
    public class UpdateBroadcastHistoryRecordCommand : AbstractUpdateCommand<BroadcastHistory>
    {
        public UpdateBroadcastHistoryRecordCommand(ICommandDb db, BroadcastHistory recToUpdate) : base(db, recToUpdate)
        {
        }

        public UpdateBroadcastHistoryRecordCommand(ICommandDb db, IList<BroadcastHistory> recsToUpdate) : base(db, recsToUpdate)
        {
        }
    }
}
