using System.Collections.Generic;
using System.Linq;

using Domain.Orm.Classes;

using iBank.Server.Utilities;

namespace iBank.Services.Implementation.Shared.WhereRoute.Helpers
{
    public class FirstOriginDestinationTextBuilder
    {
        public static string BuildText(bool useFirst, List<LanguageVariableInfo> languageVariables)
        {
            if (useFirst)
            {
                var translation = languageVariables.FirstOrDefault(x => x.VariableName.EqualsIgnoreCase("XFIRST"));
                var temp = translation == null ? "First" : translation.Translation;

                return $"({temp})";
            }

            return "";
        }
    }
}
