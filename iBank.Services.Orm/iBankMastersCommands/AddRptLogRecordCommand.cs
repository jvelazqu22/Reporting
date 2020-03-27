using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankMastersCommands
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
