using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands
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
