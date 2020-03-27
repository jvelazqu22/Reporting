using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class ShareLogic
    {
        private UserDefinedParameters _userDefinedParams;
        private List<Tuple<string, string>> TripSummaryLevel { get; set; }

        public ShareLogic(ColValRulesParams colValRulesParams)
        {
            _userDefinedParams = colValRulesParams.UserDefinedParams;
            TripSummaryLevel = colValRulesParams.TripSummaryLevelTuple;
        }

        public string FindLongestCityPair(int reckey)
        {
            if (_userDefinedParams.MarketSegmentDataList.Count == 0) return string.Empty;

            var rec = _userDefinedParams.MarketSegmentLookup[reckey].OrderByDescending(x => x.Miles)
                .FirstOrDefault();
            //return (rec != null) ? rec.Mktsegboth.Trim() : string.Empty;
            return rec?.Mktsegboth.Trim() ?? string.Empty;
        }

        public bool CalcTripSummaryField(string recloc, string colName)
        {
            if (TripSummaryLevel.Any(s => s.Item1.EqualsIgnoreCase(recloc) && s.Item2.EqualsIgnoreCase(colName))) return false;

            TripSummaryLevel.Add(new Tuple<string, string>(recloc, colName));
            return true;
        }
    }
}