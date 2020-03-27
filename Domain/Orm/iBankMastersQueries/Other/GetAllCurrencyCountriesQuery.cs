using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetAllCurrencyCountriesQuery : IQuery<IList<CurrencyCountry>>
    {
        private readonly IMastersQueryable _db;

        public GetAllCurrencyCountriesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<CurrencyCountry> ExecuteQuery()
        {
            using(_db)
            {
                return _db.CurCountry.Select(s => new CurrencyCountry
                                                      {
                                                          Recordno = s.recordno,
                                                          Country = s.country,
                                                          CurCode = s.curcode,
                                                          Symbol = s.csymbol,
                                                          LeftRight = s.cleftright,
                                                          Decimal = s.cdecimal,
                                                          Thousands = s.cthousands,
                                                          Inactive = s.inactive,
                                                          NumCurCode = s.numcurcode,
                                                          CountryName = s.CountryName,
                                                          CountryCode = s.CtryCode
                                                      }).ToList();
            }
        }
    }
}
