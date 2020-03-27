using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands
{
    public class UpdateRptLogResultsRecordCommand : AbstractUpdateCommand<ibRptLogResults>
    {
        public UpdateRptLogResultsRecordCommand(ICommandDb db, ibRptLogResults recToUpdate) : base(db, recToUpdate)
        {
        }

        public UpdateRptLogResultsRecordCommand(ICommandDb db, IList<ibRptLogResults> recsToUpdate) : base(db, recsToUpdate)
        {
        }
    }
}
