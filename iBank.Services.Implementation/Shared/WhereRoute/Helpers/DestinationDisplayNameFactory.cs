using System;

using Domain.Helper;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.Interfaces;

namespace iBank.Services.Implementation.Shared.WhereRoute.Helpers
{
    public class DestinationDisplayNameFactory : IFactory<string>
    {
        private readonly IMasterDataStore _store;

        public RoutingCriteriaType Type { get; set; }

        private readonly string _userLanguage;

        public DestinationDisplayNameFactory(RoutingCriteriaType type, IMasterDataStore store, string userLanguage)
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
                    return LookupFunctions.LookupColumnDisplayName("DESTINATION", Constants.DEFAULT_WHERE_TEXT_DISPLAY, _userLanguage, _store);
                case RoutingCriteriaType.Metro:
                    return LookupFunctions.LookupColumnDisplayName("DESTINATIONMETRO", Constants.DEFAULT_WHERE_TEXT_DISPLAY, _userLanguage, _store);
                case RoutingCriteriaType.Region:
                    return LookupFunctions.LookupColumnDisplayName("DESTREGION", Constants.DEFAULT_WHERE_TEXT_DISPLAY, _userLanguage, _store);
                case RoutingCriteriaType.Country:
                    return LookupFunctions.LookupColumnDisplayName("DEST COUNTRY", Constants.DEFAULT_WHERE_TEXT_DISPLAY, _userLanguage, _store);
                default:
                    throw new ArgumentOutOfRangeException($"Unsupported routing criteria type of [{Type}] supplied.");
            }
        }
    }
}
