using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientCommands
{
    public class AddClientBroadcastBatchHistoryCommand : AbstractAddCommand<ibbatchhistory>
    {
        public AddClientBroadcastBatchHistoryCommand(ICommandDb db, ibbatchhistory recToAdd) : base(db, recToAdd)
        {
        }

        public AddClientBroadcastBatchHistoryCommand(ICommandDb db, IList<ibbatchhistory> recToAdd) : base(db, recToAdd)
        {
        }
    }
}
