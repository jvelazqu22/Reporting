using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientCommands
{
    public class AddBcReportLogRecordCommand : AbstractAddCommand<bcreportlog>
    {
        public AddBcReportLogRecordCommand(ICommandDb db, bcreportlog recToAdd) : base(db, recToAdd) { }

        public AddBcReportLogRecordCommand(ICommandDb db, IList<bcreportlog> recsToAdd) : base(db, recsToAdd) { }
    }
}
