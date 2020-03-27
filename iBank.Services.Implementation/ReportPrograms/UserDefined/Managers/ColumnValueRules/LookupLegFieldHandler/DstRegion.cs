using iBank.Repository.SQL.Interfaces;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class DstRegion : IColumnValue
    {
        private IMasterDataStore _masterStore;
        private LegRawData _legRawData;
        private bool _isTitleCaseRequired;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _masterStore = colValRulesParams.MasterStore;
            _legRawData = colValRulesParams.LegRawData;
            _isTitleCaseRequired = colValRulesParams.IsTitleCaseRequired;
        }

        public string CalculateColValue()
        {
            //return LookupFunctions.LookupRegionCode(_legRawData.Destinat, string.Empty, _masterStore);
            return LookupFunctions.LookupRegion(_legRawData.Destinat, _legRawData.Mode, _masterStore, _isTitleCaseRequired);
        }
    }
}
