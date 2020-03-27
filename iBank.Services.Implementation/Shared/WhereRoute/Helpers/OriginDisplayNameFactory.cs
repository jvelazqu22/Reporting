using System;

using Domain.Helper;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.Interfaces;

namespace iBank.Services.Implementation.Shared.WhereRoute.Helpers
{
    public class OriginDisplayNameFactory : IFactory<string>
    {
        private readonly IMasterDataStore _store;

        public RoutingCriteriaType Type { get; set; }

        private readonly string _userLanguage;

        public OriginDisplayNameFactory(RoutingCriteriaType type, IMasterDataStore store, string userLanguage)
        {
            Type = type;
            _store = store;
            _userLanguage = userLanguage;
        }

        public string Build()
        {
            switch (Type)
            {
                case RoutingCriteriaType.Airport:
                    return LookupFunctions.LookupColumnDisplayName("ORIGIN", Constants.DEFAULT_WHERE_TEXT_DISPLAY, _userLanguage, _store);
                case RoutingCriteriaType.Metro:
                    return LookupFunctions.LookupColumnDisplayName("ORIGINMETRO", Constants.DEFAULT_WHERE_TEXT_DISPLAY, _userLanguage, _store);
                case RoutingCriteriaType.Region:
                    return LookupFunctions.LookupColumnDisplayName("ORIGINREGION", Constants.DEFAULT_WHERE_TEXT_DISPLAY, _userLanguage, _store);
                case RoutingCriteriaType.Country:
                    return LookupFunctions.LookupColumnDisplayName("ORIGINCOUNTRY", Constants.DEFAULT_WHERE_TEXT_DISPLAY, _userLanguage, _store);
                default:
                    throw new ArgumentOutOfRangeException($"Unsupported routing criteria type of [{Type}] supplied.");
            }
        }
    }
}
