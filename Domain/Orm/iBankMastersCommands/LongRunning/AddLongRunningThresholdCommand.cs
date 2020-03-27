using System.Collections.Generic;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands.LongRunning
{
    public class AddLongRunningThresholdCommand : AbstractAddCommand<BroadcastLongRunningThreshold>
    {
        public AddLongRunningThresholdCommand(ICommandDb db, BroadcastLongRunningThreshold recToAdd) : base(db, recToAdd)
        {
        }

        public AddLongRunningThresholdCommand(ICommandDb db, IList<BroadcastLongRunningThreshold> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
