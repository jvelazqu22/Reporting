using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientCommands
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
