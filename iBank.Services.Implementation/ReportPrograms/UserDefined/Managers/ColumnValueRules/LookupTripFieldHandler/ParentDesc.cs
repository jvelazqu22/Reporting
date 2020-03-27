using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.Client;
using Domain.Models.ReportPrograms.UserDefinedReport;
using Domain.Orm.iBankClientQueries;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class ParentDesc : IColumnValue
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
            return _clientFunctions.LookupParent(new GetAllMasterAccountsQuery(_clientStore.ClientQueryDb, _globals.Agency), _mainRec.Acct, 
                new GetAllParentAccountsQuery(_clientStore.ClientQueryDb), _globals.Agency).AccountDescription;
        }
    }
}
