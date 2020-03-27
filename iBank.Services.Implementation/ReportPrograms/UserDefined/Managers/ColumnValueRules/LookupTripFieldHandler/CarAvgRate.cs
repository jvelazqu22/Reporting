using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class CarAvgRate : IColumnValue
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
                var carDataTemp = _userDefinedParams.CarDataList.Where(s => recKeys.Contains(s.RecKey)).ToList();
                var days = Math.Abs(carDataTemp.Sum(s => s.Days));
                if (days == 0) return string.Empty;

                var avgRate = carDataTemp.Sum(s => Math.Abs(s.Days) * s.Abookrat / days);
                return $"{avgRate:0.00}";
            }
            var dayCount = _userDefinedParams.CarLookup[_mainRec.RecKey].Sum(x => x.Days);
            if (dayCount != 0)
            {
                var avgCost = _userDefinedParams.CarLookup[_mainRec.RecKey].Where(s => s.RecKey == _mainRec.RecKey).Sum(s => Math.Abs(s.Days) * Math.Abs(s.Abookrat) / dayCount);
                return avgCost.ToString().PadLeft(12);
            }
            return "[null]";
        }
    }
}
