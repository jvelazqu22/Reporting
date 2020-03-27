using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands.Monitoring
{
    public class AddTimeoutBroadcastCommand : AbstractAddCommand<timeout_broadcasts>
    {
        public AddTimeoutBroadcastCommand(ICommandDb db, timeout_broadcasts recToAdd) : base(db, recToAdd)
        {
        }

        public AddTimeoutBroadcastCommand(ICommandDb db, IList<timeout_broadcasts> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
