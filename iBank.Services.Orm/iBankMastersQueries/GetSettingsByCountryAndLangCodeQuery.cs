using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetSettingsByCountryAndLangCodeQuery : BaseiBankMastersQuery<InternationalSettingsInformation>
    {
        public string Country { get; set; }
        public string LanguageCode { get; set; }

        public GetSettingsByCountryAndLangCodeQuery(IMastersQueryable db, string country, string languageCode)
        {
            _db = db;
            Country = country;
            LanguageCode = string.IsNullOrEmpty(languageCode) ? "EN" : languageCode;

        }
        public override InternationalSettingsInformation ExecuteQuery()
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
