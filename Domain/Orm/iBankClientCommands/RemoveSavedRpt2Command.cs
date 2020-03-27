using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientCommands
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
