using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.Client;
using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class ClientName : IColumnValue
    {
        private ClientFunctions _clientFunctions;
        private IClientDataStore _clientStore;
        private ReportGlobals _globals;
        private RawData _mainRec;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _clientFunctions = colValRulesParams.ClientFunctions;
            _clientStore = colValRulesParams.ClientDataStore;
            _globals = colValRulesParams.Globals;
            _mainRec = colValRulesParams.MainRec;
        }

        public string CalculateColValue()
        {
            return _clientFunctions.LookupClientName(_mainRec.Clientid, _globals.Agency, _clientStore.ClientQueryDb);
        }
    }
}
