using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientCommands
{
    public class RemoveiBBatchCommand : AbstractRemoveCommand<ibbatch>
    {
        public RemoveiBBatchCommand(ICommandDb db, ibbatch recToRemove) : base(db, recToRemove)
        {
        }

        public RemoveiBBatchCommand(ICommandDb db, IList<ibbatch> recsToRemove) : base(db, recsToRemove)
        {
        }
    }
}
