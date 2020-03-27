using System.Collections.Generic;
using System.Linq;

using Domain.Interfaces;

using iBank.Server.Utilities;

namespace iBank.Services.Implementation.Shared
{
    public class CollapserHelper
    {
        private static readonly string _international = "I";

        private static readonly string _domestic = "D";

        private static readonly string _transBorder = "T";
        public static string GetDomesticInternationalType<T>(IList<T> data) where T : ICollapsible
        {
            //if there is an international leg than return international
            if(data.Any(x => x.DitCode.EqualsIgnoreCase(_international))) return _international;

            //else if there is a transborder leg return transborder
            if (data.Any(x => x.DitCode.EqualsIgnoreCase(_transBorder))) return _transBorder;

            //else return domestic
            return _domestic;
        }
    }
}
