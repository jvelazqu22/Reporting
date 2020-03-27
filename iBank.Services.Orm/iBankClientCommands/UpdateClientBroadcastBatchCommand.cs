using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankClientCommands
{
    public class UpdateClientBroadcastBatchCommand : AbstractUpdateCommand<ibbatch>
    {
        public UpdateClientBroadcastBatchCommand(ICommandDb db, ibbatch recToUpdate) : base(db, recToUpdate)
        {
        }

        public UpdateClientBroadcastBatchCommand(ICommandDb db, IList<ibbatch> recsToUpdate) : base(db, recsToUpdate)
        {
        }
    }
}
