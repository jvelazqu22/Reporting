using System.Collections.Generic;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands.PendingRecords
{
    public class RemovePendingReportsCommand : AbstractRemoveCommand<PendingReports>
    {
        public RemovePendingReportsCommand(ICommandDb db, PendingReports recToRemove) : base(db, recToRemove)
        {
        }

        public RemovePendingReportsCommand(ICommandDb db, IList<PendingReports> recsToRemove) : base(db, recsToRemove)
        {
        }
    }
}
