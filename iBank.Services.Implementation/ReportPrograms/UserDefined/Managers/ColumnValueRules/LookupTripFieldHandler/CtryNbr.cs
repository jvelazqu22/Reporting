using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class CtryNbr : IColumnValue
    {
        private RawData _mainRec;
        private IMasterDataStore _masterStore;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _masterStore = colValRulesParams.MasterStore;
        }

        public string CalculateColValue()
        {
            if (_mainRec.Invctrycod == null) return "[null]";
            return LookupFunctions.LookupCountryNumber(_masterStore, _mainRec.Invctrycod);
        }
    }
}
