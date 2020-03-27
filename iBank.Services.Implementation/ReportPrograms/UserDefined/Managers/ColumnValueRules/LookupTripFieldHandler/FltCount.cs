using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class FltCount : IColumnValue
    {
        private RawData _mainRec;
        private int _tripSummaryLevel;
        private UserReportColumnInformation _column;
        private UserDefinedParameters _userDefinedParams;
        private List<Tuple<string, string>> TripSummaryLevel { get; set; }
        private ShareLogic _sharedLogic;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _tripSummaryLevel = colValRulesParams.TripSummaryLevelInt;
            _column = colValRulesParams.Column;
            _userDefinedParams = colValRulesParams.UserDefinedParams;
            TripSummaryLevel = colValRulesParams.TripSummaryLevelTuple;
            _sharedLogic = new ShareLogic(colValRulesParams);
        }

        public string CalculateColValue()
        {
            if (_tripSummaryLevel == 1)
            {
                if (!_sharedLogic.CalcTripSummaryField(_mainRec.Recloc, _column.Name)) return string.Empty;

                var recKeys = _userDefinedParams.TripDataList.Where(s => s.Recloc.EqualsIgnoreCase(_mainRec.Recloc)).Select(s => s.RecKey);
                return _userDefinedParams.LegDataList.Count(s => recKeys.Contains(s.RecKey)).ToString();
            }
            var fltCount = _userDefinedParams.LegLookup[_mainRec.RecKey].Count();
            return fltCount.ToString();
        }
    }
}
