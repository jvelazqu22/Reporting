using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientCommands
{
    public class AddiBBatchCommand : AbstractAddCommand<ibbatch>
    {
        public AddiBBatchCommand(ICommandDb db, ibbatch recToAdd) : base(db, recToAdd)
        {
        }

        public AddiBBatchCommand(ICommandDb db, IList<ibbatch> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
