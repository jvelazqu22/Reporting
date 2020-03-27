using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands
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
