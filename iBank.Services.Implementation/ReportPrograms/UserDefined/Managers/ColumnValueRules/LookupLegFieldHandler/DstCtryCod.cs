using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class DstCtryCod : IColumnValue
    {
        private LegRawData _legRawData;
        private IMasterDataStore _masterStore;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _legRawData = colValRulesParams.LegRawData;
            _masterStore = colValRulesParams.MasterStore;
        }

        public string CalculateColValue()
        {
            return LookupFunctions.LookupCountryCode(_legRawData.Destinat, _masterStore);
        }
    }
}
