using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands
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
