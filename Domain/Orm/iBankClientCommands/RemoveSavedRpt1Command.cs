using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientCommands
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
