using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class HomCtryCod : IColumnValue
    {
        private IMasterDataStore _masterStore;
        private ReportGlobals _globals;
        private RawData _mainRec;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _masterStore = colValRulesParams.MasterStore;
            _globals = colValRulesParams.Globals;
            _mainRec = colValRulesParams.MainRec;
        }

        public string CalculateColValue()
        {
            return LookupFunctions.LookupHomeCountryCode(_mainRec.Sourceabbr, _globals, _masterStore);
        }
    }
}
