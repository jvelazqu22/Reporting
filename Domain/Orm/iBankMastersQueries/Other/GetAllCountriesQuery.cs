using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetAllCountriesQuery : IQuery<IList<CountriesInformation>>
    {
        protected IMastersQueryable _db;
        private bool _disposed = false;
        public GetAllCountriesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        ~GetAllCountriesQuery()
        {
            if (!_disposed) _db.Dispose();
        }

        public IList<CountriesInformation> ExecuteQuery()
        {
            _disposed = true;
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
