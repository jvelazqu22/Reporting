using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class OrgDesReg : IColumnValue
    {
        private RawData _mainRec;
        private IMasterDataStore _masterStore;
        private bool _isTitleCaseRequired;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _masterStore = colValRulesParams.MasterStore;
            _isTitleCaseRequired = colValRulesParams.IsTitleCaseRequired;
        }

        public string CalculateColValue()
        {
            //Alphabetic:  Derived from Origin Continent to Destination Continent (all Europe to United States trips and United States to Europe trips would show as Europe-United States)
            return LookupFunctions.LookupCityPairRegionToRegion(_mainRec.Trpctypair, _masterStore, _isTitleCaseRequired);
        }
    }
}
