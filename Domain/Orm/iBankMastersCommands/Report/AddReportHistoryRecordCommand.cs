using System.Collections.Generic;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands.Report
{
    public class AddReportHistoryRecordCommand : AbstractAddCommand<ReportHistory>
    {
        public AddReportHistoryRecordCommand(ICommandDb db, ReportHistory recToAdd) : base(db, recToAdd)
        {
        }

        public AddReportHistoryRecordCommand(ICommandDb db, IList<ReportHistory> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
