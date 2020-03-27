using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankClientCommands
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
