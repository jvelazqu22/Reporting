using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientCommands
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
