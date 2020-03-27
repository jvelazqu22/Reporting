using System.Collections.Generic;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands.PendingRecords
{
    public class UpdatePendingReportCommand : AbstractUpdateCommand<PendingReports>
    {
        public UpdatePendingReportCommand(ICommandDb db, PendingReports recToUpdate) : base(db, recToUpdate)
        {
        }

        public UpdatePendingReportCommand(ICommandDb db, IList<PendingReports> recsToUpdate) : base(db, recsToUpdate)
        {
        }
    }
}
