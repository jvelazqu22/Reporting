using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands.Monitoring
{
    public class UpdateTimeoutBroadcastCommand : AbstractAddCommand<timeout_broadcasts>
    {
        public UpdateTimeoutBroadcastCommand(ICommandDb db, timeout_broadcasts recToAdd) : base(db, recToAdd)
        {
        }

        public UpdateTimeoutBroadcastCommand(ICommandDb db, IList<timeout_broadcasts> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
