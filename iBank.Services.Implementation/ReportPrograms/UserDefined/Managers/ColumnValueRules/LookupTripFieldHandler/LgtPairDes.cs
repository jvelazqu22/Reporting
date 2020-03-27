using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class LgtPairDes : IColumnValue
    {
        private RawData _mainRec;
        private IMasterDataStore _masterStore;
        private bool _isTitleCaseRequired;
        private ReportGlobals _globals;
        private ShareLogic _sharedLogic;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _masterStore = colValRulesParams.MasterStore;
            _isTitleCaseRequired = colValRulesParams.IsTitleCaseRequired;
            _globals = colValRulesParams.Globals;
            _sharedLogic = new ShareLogic(colValRulesParams);
        }

        public string CalculateColValue()
        {
            //TODO code may need to change - new lookup function for Bi Directional O&D Description (based on longest segment)
            if (_globals.Agency.Equals("GSA"))
            {
                return LookupFunctions.LookupCityPair_GSA(_sharedLogic.FindLongestCityPair(_mainRec.RecKey), "A", "CITYDESC", _masterStore, _isTitleCaseRequired);
            }
            else
            {
                return LookupFunctions.LookupCityPair(_sharedLogic.FindLongestCityPair(_mainRec.RecKey), "A", "CITYDESC", _masterStore, _isTitleCaseRequired);
            }
        }
    }
}
