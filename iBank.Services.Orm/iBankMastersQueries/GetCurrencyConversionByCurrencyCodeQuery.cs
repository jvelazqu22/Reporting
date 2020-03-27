using System.Collections.Generic;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetCurrencyConversionByCurrencyCodeQuery : BaseiBankMastersQuery<IList<CurrencyConversionInformation>>
    {
        public string CurrencyCode { get; set; }
        
        public GetCurrencyConversionByCurrencyCodeQuery(IMastersQueryable db, string currencyCode)
        {
            _db = db;
            CurrencyCode = currencyCode;
        }

        public override IList<CurrencyConversionInformation> ExecuteQuery()
        {
            using(_db)
            {
                return _db.CurConversion.Where(x => x.curcode == CurrencyCode).OrderBy(x => x.curdate)
                    .Select(x => new CurrencyConversionInformation
                                     {
                                         CurrencyCode = x.curcode,
                                         CurrencyDate = x.curdate,
                                         USDFactor = x.usdfactor ?? 0
                                     }).ToList();
            }
        }
    }
}
