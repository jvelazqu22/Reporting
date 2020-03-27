using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankMastersCommands
{
    public class UpdateReportHandoffRecordCommand : AbstractUpdateCommand<reporthandoff>
    {
        public UpdateReportHandoffRecordCommand(ICommandDb db, reporthandoff recToUpdate) : base(db, recToUpdate)
        {
        }

        public UpdateReportHandoffRecordCommand(ICommandDb db, IList<reporthandoff> recsToUpdate) : base(db, recsToUpdate)
        {
        }
    }
}
