using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankMastersCommands
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
