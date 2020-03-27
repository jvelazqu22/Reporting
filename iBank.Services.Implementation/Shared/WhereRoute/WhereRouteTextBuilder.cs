using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Orm.Classes;

using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.Shared.WhereRoute
{
    public class WhereRouteTextBuilder
    {
        public string BuildWhereTextFromAirlines(string mode, IList<LanguageVariableInfo> languageVariables)
        {
            switch (mode)
            {
                case "1":
                    var airOnly = "Air Only";
                    var langAirOnly =
                        languageVariables.FirstOrDefault(s => s.VariableName.Equals("LL_AIRONLY", StringComparison.InvariantCultureIgnoreCase));
                    if (langAirOnly != null)
                    {
                        airOnly = langAirOnly.Translation;
                    }
                    return airOnly + ";";
                case "2":
                    var railOnly = "Rail Only";
                    var langRailOnly =
                        languageVariables.FirstOrDefault(s => s.VariableName.Equals("LL_RAILONLY", StringComparison.InvariantCultureIgnoreCase));
                    if (langRailOnly != null)
                    {
                        railOnly = langRailOnly.Translation;
                    }
                    return railOnly + ";";
            }

            return "";
        }

        public string BuildDomesticInternationalWhereText(string domIntl, bool legDIT, string userLanguage)
        {
            if (domIntl.IsBetween(2, 7) && !legDIT)
            {
                var domIntlDesc = LookupFunctions.LookupDomesticInternational(domIntl, userLanguage, new MasterDataStore());
                if (!string.IsNullOrEmpty(domIntlDesc))
                {
                    return domIntlDesc + ";";
                }
            }

            return "";
        }
    }
}
