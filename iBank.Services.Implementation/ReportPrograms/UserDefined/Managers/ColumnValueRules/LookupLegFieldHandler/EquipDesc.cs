using iBank.Repository.SQL.Interfaces;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class EquipDesc : IColumnValue
    {
        private IMasterDataStore _masterStore;
        private LegRawData _legRawData;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _legRawData = colValRulesParams.LegRawData;
            _masterStore = colValRulesParams.MasterStore;
        }

        public string CalculateColValue()
        {
            return LookupFunctions.LookupEquipment(_legRawData.Equip, _masterStore);
        }
    }
}
