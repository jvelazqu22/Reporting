using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankClientCommands
{
    public class RemoveSavedRpt3Command : AbstractRemoveCommand<savedrpt3>
    {
        public RemoveSavedRpt3Command(ICommandDb db, savedrpt3 recToRemove) : base(db, recToRemove)
        {
        }

        public RemoveSavedRpt3Command(ICommandDb db, IList<savedrpt3> recsToRemove) : base(db, recsToRemove)
        {
        }
    }
}
