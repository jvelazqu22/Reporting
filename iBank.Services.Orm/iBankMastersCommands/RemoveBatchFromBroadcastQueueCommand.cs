using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankMastersCommands
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
