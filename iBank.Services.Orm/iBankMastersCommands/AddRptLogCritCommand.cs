using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankMastersCommands
{
    public class AddRptLogCritCommand : AbstractAddCommand<ibRptLogCrit>
    {
        public AddRptLogCritCommand(ICommandDb db, ibRptLogCrit recToAdd) : base(db, recToAdd)
        {
        }

        public AddRptLogCritCommand(ICommandDb db, IList<ibRptLogCrit> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
