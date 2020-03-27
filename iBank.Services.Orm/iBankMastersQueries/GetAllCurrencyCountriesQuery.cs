using System.Collections.Generic;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetAllCurrencyCountriesQuery : BaseiBankMastersQuery<IList<CurrencyCountry>>
    {
        public GetAllCurrencyCountriesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public override IList<CurrencyCountry> ExecuteQuery()
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
