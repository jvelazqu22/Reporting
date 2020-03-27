using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankClientCommands
{
    public class AddBcReportLogRecordCommand : AbstractAddCommand<bcreportlog>
    {
        public AddBcReportLogRecordCommand(ICommandDb db, bcreportlog recToAdd) : base(db, recToAdd)
        {
        }

        public AddBcReportLogRecordCommand(ICommandDb db, IList<bcreportlog> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
