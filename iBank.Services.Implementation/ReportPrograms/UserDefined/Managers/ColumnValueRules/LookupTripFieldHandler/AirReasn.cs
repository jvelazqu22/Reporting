using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class AirReasn : IColumnValue
    {
        private IClientDataStore _clientStore;
        private ReportGlobals _globals;
        private RawData _mainRec;
        private ReportLookups _reportLookups;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _clientStore = colValRulesParams.ClientDataStore;
            _globals = colValRulesParams.Globals;
            _mainRec = colValRulesParams.MainRec;
            _reportLookups = colValRulesParams.ReportLookups;
        }

        public string CalculateColValue()
        {
            return _reportLookups.LookupReason(_mainRec.Reascode, _clientStore.ClientQueryDb, _globals.Agency, _globals.UserLanguage);
        }
    }
}
