using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankClientCommands
{
    public class UpdateUserReportCommand : AbstractUpdateCommand<userrpt>
    {
        public UpdateUserReportCommand(ICommandDb db, userrpt recToUpdate) : base(db, recToUpdate)
        {
        }

        public UpdateUserReportCommand(ICommandDb db, IList<userrpt> recsToUpdate) : base(db, recsToUpdate)
        {
        }
    }    
}
