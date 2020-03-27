using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankClientCommands
{
    public class UpdateUserCommand : AbstractUpdateCommand<ibuser>
    {
        public UpdateUserCommand(ICommandDb db, ibuser recToUpdate) : base(db, recToUpdate)
        {
        }

        public UpdateUserCommand(ICommandDb db, IList<ibuser> recsToUpdate) : base(db, recsToUpdate)
        {
        }
    }
}
