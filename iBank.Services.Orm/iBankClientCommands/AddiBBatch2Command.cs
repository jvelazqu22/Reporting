using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankClientCommands
{
    public class AddiBBatch2Command : AbstractAddCommand<ibbatch2>
    {
        public AddiBBatch2Command(ICommandDb db, ibbatch2 recToAdd) : base(db, recToAdd)
        {
        }

        public AddiBBatch2Command(ICommandDb db, IList<ibbatch2> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
