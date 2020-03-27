using System.Linq;

using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    public class BuildWhereUtilities
    {
        private const string FieldEqualsValue = "{0} = {1}; ";

        //Hope this will get used in the future so it won't be force to use either UserLanguage or OutputLanguage
        public static string GetNotInTextByLangCode(bool needTranslation, string langCode, IMasterDataStore store)
        {
            return needTranslation
                            ? $" {LookupFunctions.LookupOperator("not in", langCode, store)} "
                            : " not in ";
        }

        //Hope this will get used in the future so it won't be force to use either UserLanguage or OutputLanguage
        public static string GetInTextByLangCode(bool needTranslation, string langCode, IMasterDataStore store)
        {
            return needTranslation
                         ? $" {LookupFunctions.LookupOperator("in", langCode, store)} "
                         : " in ";
        }

        public static string GetCurrencyWhereText(string currency, ReportGlobals globals)
        {
            var curLabel = globals.LanguageVariables.FirstOrDefault(s => s.VariableName.Substring(1).EqualsIgnoreCase("Currency"));

            var currencyLabel = curLabel == null
                                    ? globals.ReportMessages.Currency
                                    : curLabel.Translation;

            return string.Format(FieldEqualsValue, currencyLabel, currency);
        }
    }
}
