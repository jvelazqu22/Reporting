using Domain.Interfaces.BroadcastServer;
using Domain.Orm.iBankClientCommands;
using iBank.Server.Utilities;

using System;

using iBank.Entities.ClientEntities;

using iBankDomain.RepositoryInterfaces;

namespace iBank.BroadcastServer.User
{
    public class UserManager : IUserManager
    {
        public ibuser User { get; set; }

        public UserManager(ibuser user)
        {
            User = user;
        }

        public void UpdateUserLogin(DateTime loginTime, ICommandDb clientCommandDb)
        {
            User.LastLogin = DateTime.Now;

            var updateUserCmd = new UpdateUserCommand(clientCommandDb, User);
            updateUserCmd.ExecuteCommand();
        }

        public string GetUserBroadcastLanguage(ibbatch originalBatch, IQuery<string> getSiteDefaultLanguage, IQuery<ibUserExtras> getUserLanguage)
        {
            var siteSettings = "[SIT]";
            var userSetting = "[USR]";
            //default to site setting
            var broadcastLanguage = (originalBatch == null) ? siteSettings : originalBatch.LangCode.Trim().ToUpper();

            if (broadcastLanguage.EqualsIgnoreCase(siteSettings))
            {
                broadcastLanguage = getSiteDefaultLanguage.ExecuteQuery();
            }

            if (broadcastLanguage.EqualsIgnoreCase(userSetting))
            {
                var userLanguage = getUserLanguage.ExecuteQuery();
                broadcastLanguage = (userLanguage == null) ? "EN" : userLanguage.FieldData.Trim();
            }

            return broadcastLanguage;
        }
    }
}
