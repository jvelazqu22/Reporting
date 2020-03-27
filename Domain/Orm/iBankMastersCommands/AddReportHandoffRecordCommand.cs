using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands
{
    public class AddReportHandoffRecordCommand : AbstractAddCommand<reporthandoff>
    {
        public AddReportHandoffRecordCommand(ICommandDb db, reporthandoff recToAdd) : base(db, recToAdd)
        {
        }

        public AddReportHandoffRecordCommand(ICommandDb db, IList<reporthandoff> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
