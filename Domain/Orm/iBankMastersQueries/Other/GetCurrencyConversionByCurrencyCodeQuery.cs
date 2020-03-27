using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetCurrencyConversionByCurrencyCodeQuery : IQuery<IList<CurrencyConversionInformation>>
    {
        public string CurrencyCode { get; set; }

        private readonly IMastersQueryable _db;
        
        public GetCurrencyConversionByCurrencyCodeQuery(IMastersQueryable db, string currencyCode)
        {
            _db = db;
            CurrencyCode = currencyCode;
        }

        public IList<CurrencyConversionInformation> ExecuteQuery()
        {
            using(_db)
            {
                return _db.CurConversion.Where(x => x.curcode == CurrencyCode)
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
