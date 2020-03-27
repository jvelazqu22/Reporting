using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientCommands
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
