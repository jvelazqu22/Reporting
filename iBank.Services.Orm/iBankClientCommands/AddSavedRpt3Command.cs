using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankClientCommands
{
    public class AddSavedRpt3Command : AbstractAddCommand<savedrpt3>
    {
        public AddSavedRpt3Command(ICommandDb db, savedrpt3 recToAdd) : base(db, recToAdd)
        {
        }

        public AddSavedRpt3Command(ICommandDb db, IList<savedrpt3> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
