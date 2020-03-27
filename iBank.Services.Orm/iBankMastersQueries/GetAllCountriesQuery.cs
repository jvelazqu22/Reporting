using System.Collections.Generic;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetAllCountriesQuery : BaseiBankMastersQuery<IList<CountriesInformation>>
    {
        public GetAllCountriesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public override IList<CountriesInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.Countries.Select(s => new CountriesInformation
                                                     {
                                                         CountryCode = s.CtryCode.Trim(),
                                                         CountryName = s.CtryName.Trim(),
                                                         NumberCountryCode = s.NumCtryCod.Trim(),
                                                         RegionCode = s.regionCode.Trim(),
                                                         LanguageCode = s.LangCode.Trim()
                                                     }).ToList();
            }
        }
    }
}
