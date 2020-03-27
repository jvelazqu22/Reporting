using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class OrgDstCont : IColumnValue
    {
        private IMasterDataStore _masterStore;
        private LegRawData _legRawData;
        private bool _isTitleCaseRequired;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _legRawData = colValRulesParams.LegRawData;
            _masterStore = colValRulesParams.MasterStore;
            _isTitleCaseRequired = colValRulesParams.IsTitleCaseRequired;
        }

        public string CalculateColValue()
        {
            return LookupFunctions.LookupOrgContinentToDestContinent(_legRawData.Origin, _legRawData.Destinat, _isTitleCaseRequired, _masterStore);
        }
    }
}
