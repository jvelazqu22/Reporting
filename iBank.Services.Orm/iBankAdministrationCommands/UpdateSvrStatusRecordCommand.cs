using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankAdministrationCommands
{
    public class UpdateSvrStatusRecordCommand : AbstractUpdateCommand<SvrStatus>
    {
        public UpdateSvrStatusRecordCommand(ICommandDb db, SvrStatus recToUpdate) : base(db, recToUpdate)
        {
        }

        public UpdateSvrStatusRecordCommand(ICommandDb db, IList<SvrStatus> recsToUpdate) : base(db, recsToUpdate)
        {
        }
    }
}
