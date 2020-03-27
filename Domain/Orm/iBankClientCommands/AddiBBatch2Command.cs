using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientCommands
{
    public class AddiBBatch2Command : AbstractAddCommand<ibbatch2>
    {
        public AddiBBatch2Command(ICommandDb db, ibbatch2 recToAdd) : base(db, recToAdd)
        {
        }

        public AddiBBatch2Command(ICommandDb db, IList<ibbatch2> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
