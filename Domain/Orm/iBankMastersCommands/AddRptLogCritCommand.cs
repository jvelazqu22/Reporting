using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersCommands
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
