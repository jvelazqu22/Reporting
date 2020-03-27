using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class LgtPair : IColumnValue
    {
        private RawData _mainRec;
        private ShareLogic _sharedLogic;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _sharedLogic = new ShareLogic(colValRulesParams);
        }

        public string CalculateColValue()
        {
            //TODO code may need to change - new lookup function for Bi Directional O&D Code (based on longest segment)
            return _sharedLogic.FindLongestCityPair(_mainRec.RecKey);
        }
    }
}
