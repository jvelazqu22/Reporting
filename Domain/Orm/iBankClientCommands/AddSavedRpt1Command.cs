using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientCommands
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
