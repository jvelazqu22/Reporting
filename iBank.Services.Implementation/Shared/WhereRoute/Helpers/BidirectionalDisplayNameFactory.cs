using System;

using Domain.Helper;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.Interfaces;

namespace iBank.Services.Implementation.Shared.WhereRoute.Helpers
{
    public class BidirectionalDisplayNameFactory : IFactory<string>
    {
        private readonly IMasterDataStore _store;

        public  RoutingCriteriaType Type { get; set; }

        private readonly string _userLanguage;

        public BidirectionalDisplayNameFactory(RoutingCriteriaType type, IMasterDataStore store, string userLanguage)
        {
            Type = type;
            _store = store;
            _userLanguage = userLanguage;
        }

        public string Build()
        {
            var origin = "";
            var destination = "";
            switch (Type)
            {
                case RoutingCriteriaType.Airport:
                    origin = LookupFunctions.LookupColumnDisplayName("ORIGIN", Constants.DEFAULT_WHERE_TEXT_DISPLAY, _userLanguage, _store);
                    destination = LookupFunctions.LookupColumnDisplayName("DESTINATION", Constants.DEFAULT_WHERE_TEXT_DISPLAY, _userLanguage, _store);
                    break;
                case RoutingCriteriaType.Metro:
                    origin = LookupFunctions.LookupColumnDisplayName("ORIGINMETRO", Constants.DEFAULT_WHERE_TEXT_DISPLAY, _userLanguage, _store);
                    destination = LookupFunctions.LookupColumnDisplayName("DESTINATIONMETRO", Constants.DEFAULT_WHERE_TEXT_DISPLAY, _userLanguage, _store);
                    break;
                case RoutingCriteriaType.Region:
                    origin = LookupFunctions.LookupColumnDisplayName("ORIGINREGION", Constants.DEFAULT_WHERE_TEXT_DISPLAY, _userLanguage, _store);
                    destination = LookupFunctions.LookupColumnDisplayName("DESTREGION", Constants.DEFAULT_WHERE_TEXT_DISPLAY, _userLanguage, _store);
                    break;
                case RoutingCriteriaType.Country:
                    origin = LookupFunctions.LookupColumnDisplayName("ORIGINCOUNTRY", Constants.DEFAULT_WHERE_TEXT_DISPLAY, _userLanguage, _store);
                    destination = LookupFunctions.LookupColumnDisplayName("DEST COUNTRY", Constants.DEFAULT_WHERE_TEXT_DISPLAY, _userLanguage, _store);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unsupported routing criteria type of [{Type}] supplied.");
            }

            return $"{origin}/{destination}";
        }
    }
}
