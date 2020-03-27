using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.Client;
using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class NetNumSegs : IColumnValue
    {
        private ClientFunctions _clientFunctions;
        private IClientDataStore _clientStore;
        private ReportGlobals _globals;
        private RawData _mainRec;
        ColValRulesParams _colValRulesParams;
        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _clientFunctions = colValRulesParams.ClientFunctions;
            _clientStore = colValRulesParams.ClientDataStore;
            _globals = colValRulesParams.Globals;
            _colValRulesParams = colValRulesParams;
            _mainRec = colValRulesParams.MainRec;
        }

        public string CalculateColValue()
        {
            if (_mainRec.Exchange)
            {
                return "0";
            }

            if (_mainRec.Trantype.Trim().Equals("I"))
            {
                return _mainRec.Numsegs.ToString() ?? string.Empty;
            }
            else if (_mainRec.Trantype.Trim().Equals("C"))
            {
                var negNumSegs = _mainRec.Numsegs * -1;
                return negNumSegs.ToString() ?? string.Empty;
            }
            return _mainRec.Numsegs.ToString() ?? string.Empty;
        }
    }
}
