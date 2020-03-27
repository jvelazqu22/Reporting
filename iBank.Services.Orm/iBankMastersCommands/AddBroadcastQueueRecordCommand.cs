using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankMastersCommands
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
