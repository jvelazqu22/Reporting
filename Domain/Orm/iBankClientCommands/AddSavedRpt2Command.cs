using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientCommands
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
