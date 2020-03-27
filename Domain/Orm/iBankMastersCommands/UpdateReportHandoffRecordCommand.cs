using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands
{
    public class UpdateReportHandoffRecordCommand : AbstractUpdateCommand<reporthandoff>
    {
        public UpdateReportHandoffRecordCommand(ICommandDb db, reporthandoff recToUpdate) : base(db, recToUpdate)
        {
        }

        public UpdateReportHandoffRecordCommand(ICommandDb db, IList<reporthandoff> recsToUpdate) : base(db, recsToUpdate)
        {
        }
    }
}
