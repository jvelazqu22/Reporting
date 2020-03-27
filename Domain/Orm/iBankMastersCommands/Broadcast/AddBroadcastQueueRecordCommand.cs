using System.Collections.Generic;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands.Broadcast
{
    public class AddBroadcastQueueRecordCommand : AbstractAddCommand<bcstque4>
    {
        public AddBroadcastQueueRecordCommand(ICommandDb db, bcstque4 recToAdd) : base(db, recToAdd)
        {
        }

        public AddBroadcastQueueRecordCommand(ICommandDb db, IList<bcstque4> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
