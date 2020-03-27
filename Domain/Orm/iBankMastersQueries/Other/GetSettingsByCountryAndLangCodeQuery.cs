using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetSettingsByCountryAndLangCodeQuery : IQuery<InternationalSettingsInformation>
    {
        public string Country { get; set; }
        public string LanguageCode { get; set; }
        private readonly IMastersQueryable _db;

        public GetSettingsByCountryAndLangCodeQuery(IMastersQueryable db, string country, string languageCode)
        {
            _db = db;
            Country = country;
            LanguageCode = string.IsNullOrEmpty(languageCode) ? "EN" : languageCode;

        }
        public InternationalSettingsInformation ExecuteQuery()
        {
            using (_db)
            {
                var intl = _db.IntlParm.FirstOrDefault(x => x.country.Trim() == Country.ToUpper().Trim() 
                                                        && x.LangCode.ToUpper().Trim() == LanguageCode.Trim());

                if (intl == null) return new InternationalSettingsInformation();

                return new InternationalSettingsInformation
                           {
                               DateFormat = intl.dateformat.Trim(),
                               DateMark = intl.datemark.Trim(),
                               Symbol = intl.csymbol.Trim(),
                               Decimal = intl.cdecimal.Trim(),
                               Position = intl.cleftright.Trim(),
                               DateDisplay = intl.datedisplay.Trim(),
                               CountryDescription = intl.CtryDesc.Trim()
                           };
            }
        }
    }
}
