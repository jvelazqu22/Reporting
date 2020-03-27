using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientCommands
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
