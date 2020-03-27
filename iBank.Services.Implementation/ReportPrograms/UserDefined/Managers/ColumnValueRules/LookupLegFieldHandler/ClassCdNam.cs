using iBank.Repository.SQL.Interfaces;
using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class ClassCdNam : IColumnValue
    {
        private bool _isTitleCaseRequired;
        private IClientDataStore _clientStore;
        private RawData _mainRec;
        private LegRawData _legRawData;
        private ReportLookups _reportLookups;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _reportLookups = colValRulesParams.ReportLookups;
            _legRawData = colValRulesParams.LegRawData;
            _mainRec = colValRulesParams.MainRec;
            _clientStore = colValRulesParams.ClientDataStore;
            _isTitleCaseRequired = colValRulesParams.IsTitleCaseRequired;
        }

        public string CalculateColValue()
        {
            return _reportLookups.LookupClassCategoryDescription(_legRawData.ClassCat, _mainRec.Agency, _clientStore.ClientQueryDb, _isTitleCaseRequired);
        }
    }
}
