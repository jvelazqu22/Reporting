using Domain.Interfaces.BroadcastServer;
using Domain.Orm.Classes;
using Domain.Orm.iBankClientQueries.BroadcastQueries;
using iBank.Server.Utilities;

using System.Collections.Generic;
using System.Linq;

using Domain.Orm.iBankMastersQueries.BroadcastQueries;

using iBank.Repository.SQL.Interfaces;

namespace iBank.BroadcastServer.User
{
    public class UserBroadcastSettings : IUserBroadcastSettings
    {
        public UserBroadcastSettings(IClientDataStore clientDataStore, IMasterDataStore masterDataStore, int? batchNumber, string agency, int userNumber)
        {
            ClientDataStore = clientDataStore;
            MasterDataStore = masterDataStore;
            BatchNumber = batchNumber;
            Agency = agency;
            UserNumber = userNumber;
        }

        private IMasterDataStore MasterDataStore { get; set; }
        private IClientDataStore ClientDataStore { get; set; }
        private string Agency { get; set; }

        private int? BatchNumber { get; set; }
        private int UserNumber { get; set; }

        public string BroadcastLanguage { get; set; }
        public bool ViewLogging { get; set; }
        public bool IsLogInRequired { get; set; }
        public string StyleTextHeader { get; set; }
        public string StyleHtmlHeader { get; set; }
        public string StyleTextFooter { get; set; }
        public string StyleHtmlFooter { get; set; }

        public List<LanguageVariableInfo> LanguageVariables { get; set; }
        
        public string GetLanguageTranslation(string key)
        {
            var translation = LanguageVariables.FirstOrDefault(s => s.VariableName.EqualsIgnoreCase(key));
            return translation == null ? string.Empty : translation.Translation;
        }
        public void SetLanguageTranslation(string key, string newValue)
        {
            var translation = LanguageVariables.FirstOrDefault(s => s.VariableName.EqualsIgnoreCase(key));
            if (translation != null)
                translation.Translation = newValue;
        }
        
        public void SetBroadcastLogging()
        {
            var isBroadcastLoggingOnQuery = new IsBroadcastViewLoggingOnQuery(MasterDataStore.MastersQueryDb, Agency);
            ViewLogging = isBroadcastLoggingOnQuery.ExecuteQuery();

            if (ViewLogging)
            {
                var isSecurityOnQuery = new IsBroadcastLogOnSecurityOnQuery(MasterDataStore.MastersQueryDb, Agency);
                IsLogInRequired = isSecurityOnQuery.ExecuteQuery();
            }
            else
            {
                IsLogInRequired = false;
            }
        }

        public void SetBroadcastStyle(int styleGroupNumber)
        {
            StyleTextHeader = string.Empty;
            StyleHtmlHeader = string.Empty;
            if (BroadcastLanguage.Equals("EN"))
            {
                StyleTextHeader = "Your iBank Broadcast Report has processed.";
                StyleHtmlHeader = "Click on the link^plural_s^ below to view your ^bc_offline^^plural_s^:";
            }

            StyleTextFooter = string.Empty;
            StyleHtmlFooter = string.Empty;

            var styleGroupExtrasQuery = new GetBroadcastStyleGroupQuery(ClientDataStore.ClientQueryDb, BroadcastLanguage, styleGroupNumber);
            var styleGroupExtras = styleGroupExtrasQuery.ExecuteQuery();

            foreach (var extra in styleGroupExtras)
            {
                if (extra.FieldFunction.EqualsIgnoreCase("STDBCASTMSGTEXT"))
                {
                    StyleTextFooter = extra.FieldData.Trim();
                }
                if (extra.FieldFunction.EqualsIgnoreCase("HTMLBCASTMSGTEXT"))
                {
                    StyleHtmlFooter = extra.FieldData.Trim();
                }
                if (extra.FieldFunction.EqualsIgnoreCase("STDBCASTHDGMSGTEXT"))
                {
                    StyleTextHeader = extra.FieldData.Trim();
                }
                if (extra.FieldFunction.EqualsIgnoreCase("HTMLBCASTHDGMSGTEXT"))
                {
                    StyleHtmlHeader = extra.FieldData.Trim();
                }
            }
        }

        public void SetLanguageVariables()
        {
            var getLanguageTranslationsQuery = new GetBroadcastLanguageTranslationsQuery(MasterDataStore.MastersQueryDb, BroadcastLanguage);
            LanguageVariables = getLanguageTranslationsQuery.ExecuteQuery().ToList();
        }

    }
}
