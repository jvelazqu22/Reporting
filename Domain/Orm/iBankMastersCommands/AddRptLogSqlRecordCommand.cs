using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands
{
    public class AddRptLogSqlRecordCommand : AbstractAddCommand<ibRptLogSQL>
    {
        public AddRptLogSqlRecordCommand(ICommandDb db, ibRptLogSQL recToAdd) : base(db, recToAdd)
        {
        }

        public AddRptLogSqlRecordCommand(ICommandDb db, IList<ibRptLogSQL> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
