using System.Collections.Generic;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands.Broadcast
{
    public class AddBroadcastHistoryRecordCommand : AbstractAddCommand<BroadcastHistory>
    {
        public AddBroadcastHistoryRecordCommand(ICommandDb db, BroadcastHistory recToAdd) : base(db, recToAdd)
        {
        }

        public AddBroadcastHistoryRecordCommand(ICommandDb db, IList<BroadcastHistory> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
