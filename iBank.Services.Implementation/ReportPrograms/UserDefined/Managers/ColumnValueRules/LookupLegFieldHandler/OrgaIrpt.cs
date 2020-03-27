using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class OrgaIrpt : IColumnValue
    {
        private IMasterDataStore _masterStore;
        private LegRawData _legRawData;
        private ReportGlobals _globals;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _masterStore = colValRulesParams.MasterStore;
            _legRawData = colValRulesParams.LegRawData;
            _globals = colValRulesParams.Globals;
        }

        public string CalculateColValue()
        {
            return AportLookup.LookupAport(_masterStore, _legRawData.Origin, _legRawData.Mode, _globals.Agency);
        }
    }
}
