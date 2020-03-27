using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands
{
    public class UpdateRptLogRecordCommand : AbstractUpdateCommand<ibRptLog>
    {
        public UpdateRptLogRecordCommand(ICommandDb db, ibRptLog recToUpdate) : base(db, recToUpdate)
        {
        }

        public UpdateRptLogRecordCommand(ICommandDb db, IList<ibRptLog> recsToUpdate) : base(db, recsToUpdate)
        {
        }
    }
}
