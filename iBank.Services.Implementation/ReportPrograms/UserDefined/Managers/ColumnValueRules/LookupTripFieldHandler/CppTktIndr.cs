using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class CppTktIndr : IColumnValue
    {
        private RawData _mainRec;
        private ReportLookups _reportLookups;
        private UserDefinedParameters _userDefinedParams;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _reportLookups = colValRulesParams.ReportLookups;
            _userDefinedParams = colValRulesParams.UserDefinedParams;
        }

        public string CalculateColValue()
        {
            //need to check each segment in this trip see if there is segment that matches the specific class
            var helper = new CppTicketIndicatorHelper();
            return helper.FindIndicator(_mainRec.RecKey, _reportLookups, _userDefinedParams.MarketSegmentDataList);
        }
    }
}
