using System.Collections.Generic;
using iBank.Entities.AdministrationEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankAdministrationCommands
{
    public class UpdateConfigurationCommand : AbstractUpdateCommand<ReportingConfiguration>
    {
        public UpdateConfigurationCommand(ICommandDb db, ReportingConfiguration recToUpdate) : base(db, recToUpdate)
        {
        }

        public UpdateConfigurationCommand(ICommandDb db, IList<ReportingConfiguration> recsToUpdate) : base(db, recsToUpdate)
        {
        }
    }
}
