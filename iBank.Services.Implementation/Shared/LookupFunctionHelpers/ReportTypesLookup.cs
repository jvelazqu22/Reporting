using System;
using System.Collections.Generic;

using Domain.Helper;
using Domain.Orm.Classes;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using Domain.Services;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Services.Implementation.Shared.LookupFunctionHelpers
{
    public static class ReportTypesLookup
    {
        private static readonly CacheKeys _key = CacheKeys.ReportTypesLookup;

        public static List<ProcessInformation> GetReportTypes(ICacheService cache, IMastersQueryable db)
        {
            List<ProcessInformation> reportTypes;
            if (!cache.TryGetValue(_key, out reportTypes))
            {
                var query = new GetAllReportTypesQuery(db);
                reportTypes = query.ExecuteQuery();
                cache.Set(_key, reportTypes, DateTime.Now.AddDays(1));
            }

            return reportTypes;
        }
    }
}
