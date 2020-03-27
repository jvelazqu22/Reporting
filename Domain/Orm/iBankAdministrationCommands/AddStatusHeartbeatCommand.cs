using System.Collections.Generic;

using iBank.Entities.AdministrationEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankAdministrationCommands

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
