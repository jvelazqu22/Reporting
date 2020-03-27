using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankClientCommands
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
