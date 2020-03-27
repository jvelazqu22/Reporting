using Domain.Interfaces.Database;
using iBank.Services.Orm.Interfaces;
using System.Collections.Generic;

namespace iBank.Services.Orm.iBankMastersCommands
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
