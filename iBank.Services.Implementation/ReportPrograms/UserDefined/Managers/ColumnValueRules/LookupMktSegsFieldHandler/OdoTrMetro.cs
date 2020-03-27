using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupMktSegsFieldHandler
{
    public class OdoTrMetro : IColumnValue
    {
        private MarketSegmentRawData _marketSegmentRawData;
        private IMasterDataStore _masterStore;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _marketSegmentRawData = colValRulesParams.MarketSegmentRawData;
            _masterStore = colValRulesParams.MasterStore;
        }

        public string CalculateColValue()
        {
            //cbuf2 = "luMetroCode(curMktsegs.segdest,'A')"
            return LookupFunctions.LookupMetroCode(_masterStore, _marketSegmentRawData.Segdest, "A");
        }
    }
}
