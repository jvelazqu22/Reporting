using System.Collections.Generic;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands.Report
{
    public class UpdateReportHistoryRecordCommand : AbstractUpdateCommand<ReportHistory>
    {
        public UpdateReportHistoryRecordCommand(ICommandDb db, ReportHistory recToUpdate) : base(db, recToUpdate)
        {
        }

        public UpdateReportHistoryRecordCommand(ICommandDb db, IList<ReportHistory> recsToUpdate) : base(db, recsToUpdate)
        {
        }
    }
}
