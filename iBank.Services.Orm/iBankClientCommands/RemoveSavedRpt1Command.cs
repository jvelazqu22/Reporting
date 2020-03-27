using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankClientCommands
{
    public class RemoveSavedRpt1Command : AbstractRemoveCommand<savedrpt1>
    {
        public RemoveSavedRpt1Command(ICommandDb db, savedrpt1 recToRemove) : base(db, recToRemove)
        {
        }

        public RemoveSavedRpt1Command(ICommandDb db, IList<savedrpt1> recsToRemove) : base(db, recsToRemove)
        {
        }
    }
}
