﻿using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class LbsFareTax : IColumnValue
    {
        private LegRawData _legRawData;
        private UserDefinedParameters _userDefinedParams;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _legRawData = colValRulesParams.LegRawData;
            _userDefinedParams = colValRulesParams.UserDefinedParams;
        }

        public string CalculateColValue()
        {
            return LookupHelpers.GetTaxesBreakoutByLegs(_legRawData.RecKey, _userDefinedParams.TripDataList, _userDefinedParams.LegDataList);
        }
    }
}
