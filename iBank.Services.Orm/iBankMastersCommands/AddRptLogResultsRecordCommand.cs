using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankMastersCommands
{
    public class AddRptLogResultsRecordCommand : AbstractAddCommand<ibRptLogResults>
    {
        public AddRptLogResultsRecordCommand(ICommandDb db, ibRptLogResults recToAdd) : base(db, recToAdd)
        {
        }

        public AddRptLogResultsRecordCommand(ICommandDb db, IList<ibRptLogResults> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
