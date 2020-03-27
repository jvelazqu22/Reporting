using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankMastersCommands
{
    public class UpdateRptLogRecordCommand : AbstractUpdateCommand<ibRptLog>
    {
        public UpdateRptLogRecordCommand(ICommandDb db, ibRptLog recToUpdate) : base(db, recToUpdate)
        {
        }

        public UpdateRptLogRecordCommand(ICommandDb db, IList<ibRptLog> recsToUpdate) : base(db, recsToUpdate)
        {
        }
    }
}
