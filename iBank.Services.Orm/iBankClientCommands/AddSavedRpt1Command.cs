using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankClientCommands
{
    public class AddSavedRpt1Command : AbstractAddCommand<savedrpt1>
    {
        public AddSavedRpt1Command(ICommandDb db, savedrpt1 recToAdd) : base(db, recToAdd)
        {
        }

        public AddSavedRpt1Command(ICommandDb db, IList<savedrpt1> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
