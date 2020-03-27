using System.Collections.Generic;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands.LongRunning
{
    public class AddLongRunningInstanceRecordCommand : AbstractAddCommand<BroadcastLongRunningLog>
    {
        public AddLongRunningInstanceRecordCommand(ICommandDb db, BroadcastLongRunningLog recToAdd) : base(db, recToAdd)
        {
        }

        public AddLongRunningInstanceRecordCommand(ICommandDb db, IList<BroadcastLongRunningLog> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
