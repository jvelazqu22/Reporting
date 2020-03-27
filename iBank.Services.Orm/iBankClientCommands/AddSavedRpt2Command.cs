using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankClientCommands
{
    public class AddSavedRpt2Command : AbstractAddCommand<savedrpt2>
    {
        public AddSavedRpt2Command(ICommandDb db, savedrpt2 recToAdd) : base(db, recToAdd)
        {
        }

        public AddSavedRpt2Command(ICommandDb db, IList<savedrpt2> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
