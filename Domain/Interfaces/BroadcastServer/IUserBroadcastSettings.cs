using Domain.Orm.Classes;

using System.Collections.Generic;

namespace Domain.Interfaces.BroadcastServer
{
    public interface IUserBroadcastSettings
    {
        string BroadcastLanguage { get; set; }
        bool ViewLogging { get; set; }
        bool IsLogInRequired { get; set; }
        string StyleTextHeader { get; set; }
        string StyleHtmlHeader { get; set; }
        string StyleTextFooter { get; set; }
        string StyleHtmlFooter { get; set; }

        List<LanguageVariableInfo> LanguageVariables { get; set; }

        string GetLanguageTranslation(string key);

        void SetLanguageTranslation(string key, string newValue);

        void SetBroadcastLogging();

        void SetBroadcastStyle(int styleGroupNumber);

        void SetLanguageVariables();

    }
}
