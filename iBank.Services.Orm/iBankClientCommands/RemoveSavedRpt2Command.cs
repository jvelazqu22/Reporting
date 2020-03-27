using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankClientCommands
{
    public class RemoveSavedRpt2Command : AbstractRemoveCommand<savedrpt2>
    {
        public RemoveSavedRpt2Command(ICommandDb db, savedrpt2 recToRemove) : base(db, recToRemove)
        {
        }

        public RemoveSavedRpt2Command(ICommandDb db, IList<savedrpt2> recsToRemove) : base(db, recsToRemove)
        {
        }
    }
}
