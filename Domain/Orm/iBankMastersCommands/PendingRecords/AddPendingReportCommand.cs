using System.Collections.Generic;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands.PendingRecords
{
    public class AddPendingReportCommand : AbstractAddCommand<PendingReports>
    {
        public AddPendingReportCommand(ICommandDb db, PendingReports recToAdd) : base(db, recToAdd)
        {
        }

        public AddPendingReportCommand(ICommandDb db, IList<PendingReports> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
