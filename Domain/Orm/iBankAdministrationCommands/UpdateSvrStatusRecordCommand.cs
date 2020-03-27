using System.Collections.Generic;

using iBank.Entities.AdministrationEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankAdministrationCommands
{
    public class UpdateSvrStatusRecordCommand : AbstractUpdateCommand<SvrStatus>
    {
        public UpdateSvrStatusRecordCommand(ICommandDb db, SvrStatus recToUpdate) : base(db, recToUpdate)
        {
        }

        public UpdateSvrStatusRecordCommand(ICommandDb db, IList<SvrStatus> recsToUpdate) : base(db, recsToUpdate)
        {
        }
    }
}
