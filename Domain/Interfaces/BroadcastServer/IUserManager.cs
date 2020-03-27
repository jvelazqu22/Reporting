using System;

using iBank.Entities.ClientEntities;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Interfaces.BroadcastServer
{
    public interface IUserManager
    {
        ibuser User { get; set; }
        void UpdateUserLogin(DateTime loginTime, ICommandDb clientCommandDb);

        string GetUserBroadcastLanguage(ibbatch originalBatch, IQuery<string> getSiteDefaultLanguage, IQuery<ibUserExtras> getUserLanguage);
    }
}
