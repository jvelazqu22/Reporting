using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankMastersCommands
{
    public class RemoveReportFromRunningReportsCommand : AbstractRemoveCommand<ibRunningRpts>
    {
        public RemoveReportFromRunningReportsCommand(ICommandDb db, ibRunningRpts recToRemove) : base(db, recToRemove)
        {
        }

        public RemoveReportFromRunningReportsCommand(ICommandDb db, IList<ibRunningRpts> recsToRemove) : base(db, recsToRemove)
        {
        }
    }
}
