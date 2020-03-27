using System.Collections.Generic;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.iBankMastersCommands
{
    public class AddErrorLogRecordCommand : AbstractAddCommand<errorlog>
    {
        public AddErrorLogRecordCommand(ICommandDb db, errorlog recToAdd) : base(db, recToAdd)
        {
        }

        public AddErrorLogRecordCommand(ICommandDb db, IList<errorlog> recsToAdd) : base(db, recsToAdd)
        {
        }
    }
}
