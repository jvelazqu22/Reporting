using Domain.Models.BroadcastServer;
using Domain.Interfaces.BroadcastServer;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace iBank.BroadcastServer.Email
{
    public abstract class AbstractEmailBuilder
    {
        protected BroadcastServerInformation BcstServerConfig { get; set; }
        protected bool IsOfflineReport { get; set; }
        protected IUserBroadcastSettings BcstSettings { get; set; }

        private IMastersQueryable _masterQueryDb;
        protected IMastersQueryable MasterQueryDb {
            get
            {
                return _masterQueryDb.Clone() as IMastersQueryable;
            }
            set
            {
                _masterQueryDb = value;
            }
        }

        private ICommandDb _masterCommandDb;

        protected ICommandDb MasterCommandDb
        {
            get
            {
                return _masterCommandDb.Clone() as ICommandDb;
            }
            set
            {
                _masterCommandDb = value;
            }
        }

        protected AbstractEmailBuilder(BroadcastServerInformation bcstServerConfig, bool isOfflineReport, IMastersQueryable masterQueryDb, ICommandDb masterCommandDb, IUserBroadcastSettings bcstSettings)
        {
            BcstServerConfig = bcstServerConfig;
            IsOfflineReport = isOfflineReport;
            MasterQueryDb = masterQueryDb;
            MasterCommandDb = masterCommandDb;
            BcstSettings = bcstSettings;
        }

        protected virtual string GetReplacedLinkPlaceHolder(string htmlEmail)
        {
            htmlEmail = htmlEmail.Replace("<a href=\"", "<b>");
            htmlEmail = htmlEmail.Replace("^rpt_url^", string.Empty);
            htmlEmail = htmlEmail.Replace("\" target=\"_new\">", string.Empty);
            htmlEmail = htmlEmail.Replace("</a>", "</b>");
            return htmlEmail;
        }
    }
}
