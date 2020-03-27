using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankAdministrationCommands
{
    public class AddStatusHeartbeatCommand : AbstractAddCommand<SvrStatus>
    {
        public AddStatusHeartbeatCommand(ICommandDb db, SvrStatus recToAdd) : base(db, recToAdd)
        {
        }

        public AddStatusHeartbeatCommand(ICommandDb db, IList<SvrStatus> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
