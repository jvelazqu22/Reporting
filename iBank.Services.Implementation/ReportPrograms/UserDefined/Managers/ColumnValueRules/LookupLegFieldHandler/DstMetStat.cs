using iBank.Repository.SQL.Interfaces;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class DstMetStat : IColumnValue
    {
        private IMasterDataStore _masterStore;
        private LegRawData _legRawData;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _masterStore = colValRulesParams.MasterStore;
            _legRawData = colValRulesParams.LegRawData;
        }

        public string CalculateColValue()
        {
            return LookupFunctions.LookupAirMetro(_masterStore, _legRawData.Destinat, "S");
        }
    }
}
