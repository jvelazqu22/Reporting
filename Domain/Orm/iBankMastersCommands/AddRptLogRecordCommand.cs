using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands
{
    public class AddRptLogRecordCommand : AbstractAddCommand<ibRptLog>
    {
        public AddRptLogRecordCommand(ICommandDb db, ibRptLog recToAdd) : base(db, recToAdd)
        {
        }

        public AddRptLogRecordCommand(ICommandDb db, IList<ibRptLog> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
