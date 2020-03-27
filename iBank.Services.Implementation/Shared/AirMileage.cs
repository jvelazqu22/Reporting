using System;

using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;

using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Models.ReportPrograms.AccountSummary;
using Domain.Services;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.Shared
{
    public static class AirMileageCalculator<T> where T : class, IAirMileage
    {
        private static readonly ICacheService _cache = new CacheService();
        public static void CalculateAirMileageFromTable(List<T> rawData)
        {
            if (!rawData.Any()) return;

            foreach (var row in rawData)
            {
                if (!row.Mode.Trim().EqualsIgnoreCase("R"))
                {
                    row.Miles = LookupAirMileage(row.Origin.Left(3), row.Destinat.Left(3), row.Miles);
                }
            }
        }
        
        public static int LookupAirMileage(string org, string dest, int miles)
        {
            var airMileage = AirMileageLookup.GetAirMileages(_cache, new CisMastersQueryable());

            var mileage = 0;
            if (airMileage.TryGetValue(Tuple.Create(org.Trim(), dest.Trim()), out mileage))
            {
                return mileage;
            }
            else
            {
                return miles;
            }
        }
    }

}
