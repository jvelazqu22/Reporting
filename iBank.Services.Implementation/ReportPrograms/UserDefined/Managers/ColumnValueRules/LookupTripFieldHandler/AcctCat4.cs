using Domain.Models.ReportPrograms.UserDefinedReport;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class AcctCat4 : IColumnValue
    {
        private RawData _mainRec;
        private IClientDataStore _clientStore;
        private ReportGlobals _globals;
        private ClientFunctions _clientFunctions;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _clientStore = colValRulesParams.ClientDataStore;
            _clientFunctions = colValRulesParams.ClientFunctions;
            _globals = colValRulesParams.Globals;
        }

        public string CalculateColValue()
        {
            return LookupFunctions.LookupAccountCategory(_clientFunctions, new GetAllMasterAccountsQuery(_clientStore.ClientQueryDb, _globals.Agency), _mainRec.Acct, 4);
        }
    }
}
