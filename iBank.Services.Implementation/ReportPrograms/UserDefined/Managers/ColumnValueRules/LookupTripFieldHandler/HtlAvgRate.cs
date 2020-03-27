using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class HtlAvgRate : IColumnValue
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
                var temp = _userDefinedParams.HotelDataList.Where(s => recKeys.Contains(s.RecKey)).ToList();
                var nights = Math.Abs(temp.Sum(s => s.Nights));
                if (nights == 0) return string.Empty;

                var avgRate = temp.Sum(s => Math.Abs(s.Nights) * s.Bookrate / nights);
                return avgRate.ToString("0.00");
            }
            var nightCount = _userDefinedParams.HotelLookup[_mainRec.RecKey].Sum(x => x.Nights);
            if (nightCount != 0)
            {
                var avgCost = _userDefinedParams.HotelLookup[_mainRec.RecKey].Sum(s => (Math.Abs(s.Rooms) * Math.Abs(s.Nights) * Math.Abs(s.Bookrate)) / nightCount);
                return $"{avgCost:0.00}".PadLeft(12);
            }
            return "[null]";
        }
    }
}
