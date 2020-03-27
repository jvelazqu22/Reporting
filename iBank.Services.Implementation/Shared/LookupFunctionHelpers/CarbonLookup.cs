using System;

using Domain.Helper;
using Domain.Orm.CISMastersQueries.CarbonCalculation;
using Domain.Services;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared.CarbonCalculations;
using iBank.Services.Implementation.Utilities;

namespace iBank.Services.Implementation.Shared.LookupFunctionHelpers
{
    public class CarbonLookup
    {
        private static ICacheService _cache;

        private static ICisMastersDataStore _cisMasterStore;

        private static readonly CacheKeys _key = CacheKeys.CarbonValuesLookup;

        public virtual CarbonValue Carbon
        {
            get
            {
                CarbonValue temp;
                if (!_cache.TryGetValue(_key, out temp))
                {
                    temp = FillCarbonValues();
                    _cache.Set(_key, temp, DateTime.Now.AddDays(1));
                }

                return temp;
            }
        }
        
        public CarbonLookup()
        {
            _cache = new CacheService();
            _cisMasterStore  = new CisMasterDataStore();
        }

        public CarbonLookup(ICacheService cache, ICisMastersDataStore store)
        {
            _cache = cache;
            _cisMasterStore = store;
        }
        
        public decimal GetEngineCarbon(int milesPerDay, int days, int plusMinus, bool isLargeEngine)
        {
            var rate = isLargeEngine ? Carbon.LargeEngineRate : Carbon.MediumEngineRate;
            return MathHelper.Round(milesPerDay * days * plusMinus * rate, 1);
        }

        public decimal GetHotelStayCarbon(int nights, int rooms, int plusMinus)
        {
            return MathHelper.Round(nights * rooms * plusMinus * Carbon.LbsPerHotelNightRate, 0);
        }

        public decimal GetAltCarCarbon(int miles)
        {
            return MathHelper.Round(Carbon.AltCarRate * miles, 1);
        }

        public decimal GetCarbonPounds(decimal lbsPerMile, int miles)
        {
            return MathHelper.Round(lbsPerMile * Carbon.UpliftFactor * miles, 1);
        }

        public decimal GetAltRailCarbon(int miles, string domesticInternationalCode)
        {
            var rate = domesticInternationalCode.EqualsIgnoreCase("D") ? Carbon.AltRailDomesticRate : Carbon.AltRailIntlRate;
            
            return MathHelper.Round(rate * miles, 1);
        }

        private CarbonValue FillCarbonValues()
        {
            var rateQuery = new GetAllCarbonRatesQuery(_cisMasterStore.CisMastersQueryDb);
            var rates = rateQuery.ExecuteQuery();

            var haulQuery = new GetAllCarbonHaulsQuery(_cisMasterStore.CisMastersQueryDb);
            var hauls = haulQuery.ExecuteQuery();

            return new CarbonValue(rates, hauls);
        }

    }
}
