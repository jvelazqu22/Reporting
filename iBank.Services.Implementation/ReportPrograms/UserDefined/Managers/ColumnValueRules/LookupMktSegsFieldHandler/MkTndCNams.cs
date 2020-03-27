using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupMktSegsFieldHandler
{
    public class MkTndCNams : IColumnValue
    {
        private MarketSegmentRawData _marketSegmentRawData;
        private IMasterDataStore _masterStore;
        private bool _isTitleCaseRequired;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _marketSegmentRawData = colValRulesParams.MarketSegmentRawData;
            _masterStore = colValRulesParams.MasterStore;
            _isTitleCaseRequired = colValRulesParams.IsTitleCaseRequired;
        }

        public string CalculateColValue()
        {
            //cbuf2 = "luCityPair(curMktsegs.mktsegboth,'A','CITY')"
            return LookupFunctions.LookupCityPair(_marketSegmentRawData.Mktsegboth, "A", "CITY", _masterStore, _isTitleCaseRequired);
        }
    }
}
