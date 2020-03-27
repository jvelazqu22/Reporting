using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankMastersCommands
{
    public class AddReportHandoffRecordCommand : AbstractAddCommand<reporthandoff>
    {
        public AddReportHandoffRecordCommand(ICommandDb db, reporthandoff recToAdd) : base(db, recToAdd)
        {
        }

        public AddReportHandoffRecordCommand(ICommandDb db, IList<reporthandoff> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
