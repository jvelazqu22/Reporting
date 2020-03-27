using Domain.Orm.iBankClientCommands;

using System;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public interface IUserReportUpdater
    {
        void UpdateLastUsed(userrpts userReportRecord, IClientDataStore clientStore);
    }

    public class UserReportUpdater : IUserReportUpdater
    {
        public void UpdateLastUsed(userrpts userReportRecord, IClientDataStore clientStore)
        {
            userReportRecord.lastused = DateTime.Now;
            var updateUserReportCmd = new UpdateUserReportCommand(clientStore.ClientCommandDb, userReportRecord);
            updateUserReportCmd.ExecuteCommand();
        }
    }
}
