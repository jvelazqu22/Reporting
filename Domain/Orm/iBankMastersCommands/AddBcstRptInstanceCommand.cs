using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands
{
    public class AddBcstRptInstanceCommand : AbstractAddCommand<bcstrptinstance>
    {
        public AddBcstRptInstanceCommand(ICommandDb db, bcstrptinstance recToAdd) : base(db, recToAdd)
        {
        }

        public AddBcstRptInstanceCommand(ICommandDb db, IList<bcstrptinstance> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
