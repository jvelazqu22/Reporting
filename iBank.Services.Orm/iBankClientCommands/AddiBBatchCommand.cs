using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankClientCommands
{
    public class AddiBBatchCommand : AbstractAddCommand<ibbatch>
    {
        public AddiBBatchCommand(ICommandDb db, ibbatch recToAdd) : base(db, recToAdd)
        {
        }

        public AddiBBatchCommand(ICommandDb db, IList<ibbatch> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
