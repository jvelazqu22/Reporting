using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientCommands
{
    public class UpdateUserReportCommand : AbstractUpdateCommand<userrpts>
    {
        public UpdateUserReportCommand(ICommandDb db, userrpts recToUpdate) : base(db, recToUpdate)
        {
        }

        public UpdateUserReportCommand(ICommandDb db, IList<userrpts> recsToUpdate) : base(db, recsToUpdate)
        {
        }
    }    
}
