using System.Collections.Generic;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands.Broadcast
{
    public class UpdateBroadcastQueueRecordCommand : AbstractUpdateCommand<bcstque4>
    {
        public UpdateBroadcastQueueRecordCommand(ICommandDb db, bcstque4 recToUpdate) : base(db, recToUpdate)
        {
        }

        public UpdateBroadcastQueueRecordCommand(ICommandDb db, IList<bcstque4> recsToUpdate) : base(db, recsToUpdate)
        {
        }
    }
}
