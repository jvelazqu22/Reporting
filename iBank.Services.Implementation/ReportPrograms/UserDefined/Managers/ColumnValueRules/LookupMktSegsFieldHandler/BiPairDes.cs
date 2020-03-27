using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupMktSegsFieldHandler
{
    public class BiPairDes : IColumnValue
    {
        private MarketSegmentRawData _marketSegmentRawData;
        private bool _isTitleCaseRequired;
        private ReportGlobals _globals;
        private IMasterDataStore _masterStore;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _marketSegmentRawData = colValRulesParams.MarketSegmentRawData;
            _globals = colValRulesParams.Globals;
            _masterStore = colValRulesParams.MasterStore;
            _isTitleCaseRequired = colValRulesParams.IsTitleCaseRequired;
        }

        public string CalculateColValue()
        {
            //TODO code may need to change - new lookup function for Bi Directional O&D Code 
            if (_globals.Agency.Equals("GSA"))
            {
                return LookupFunctions.LookupCityPair_GSA(_marketSegmentRawData.Mktsegboth, "A", "CITYDESC", _masterStore, _isTitleCaseRequired);
            }
            else
            {
                return LookupFunctions.LookupCityPair(_marketSegmentRawData.Mktsegboth, "A", "CITYDESC", _masterStore, _isTitleCaseRequired);
            }
        }
    }
}
