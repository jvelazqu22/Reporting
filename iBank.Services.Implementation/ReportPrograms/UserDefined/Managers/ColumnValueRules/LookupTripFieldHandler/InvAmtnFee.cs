﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class InvAmtnFee : IColumnValue
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
                var invAmtSum = _userDefinedParams.TripDataList.Where(s => s.Recloc.EqualsIgnoreCase(_mainRec.Recloc)).Sum(s => s.PlusMin);
                var svcFeeRecs = _userDefinedParams.ServiceFeeDataList.Where(s => recKeys.Contains(s.RecKey)).ToList();
                if (!svcFeeRecs.Any()) return string.Empty;

                var svcFeeSum = svcFeeRecs.Sum(s => s.Svcfee);
                return (invAmtSum - svcFeeSum).ToString("0.00");
            }
            var invAmt = _userDefinedParams.TripLookup[_mainRec.RecKey].Where(x => x.Invamt.HasValue).Sum(x => x.Invamt ?? 0);
            var svcFees = _userDefinedParams.ServiceFeeLookup[_mainRec.RecKey].Sum(x => x.Svcfee);

            return (invAmt - svcFees).ToString("0.00").PadLeft(12);
        }
    }
}
