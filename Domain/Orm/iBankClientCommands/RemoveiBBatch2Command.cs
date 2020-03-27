using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientCommands
{
    public class RemoveiBBatch2Command : AbstractRemoveCommand<ibbatch2>
    {
        public RemoveiBBatch2Command(ICommandDb db, ibbatch2 recToRemove) : base(db, recToRemove)
        {
        }

        public RemoveiBBatch2Command(ICommandDb db, IList<ibbatch2> recsToRemove) : base(db, recsToRemove)
        {
        }
    }
}
