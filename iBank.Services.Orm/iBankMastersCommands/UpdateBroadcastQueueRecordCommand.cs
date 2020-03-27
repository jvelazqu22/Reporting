using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankMastersCommands
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
