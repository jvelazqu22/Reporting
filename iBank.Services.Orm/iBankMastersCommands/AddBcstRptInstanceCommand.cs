using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankMastersCommands
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
