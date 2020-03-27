using iBank.Repository.SQL.Interfaces;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class OrgCountry : IColumnValue
    {
        private IMasterDataStore _masterStore;
        private bool _isTitleCaseRequired;
        private LegRawData _legRawData;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _legRawData = colValRulesParams.LegRawData;
            _isTitleCaseRequired = colValRulesParams.IsTitleCaseRequired;
            _masterStore = colValRulesParams.MasterStore;
        }

        public string CalculateColValue()
        {
            return LookupFunctions.LookupCountry(_masterStore, _legRawData.Origin, _isTitleCaseRequired);
        }
    }
}
