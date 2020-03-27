using iBank.Repository.SQL.Interfaces;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class CurSymbol : IColumnValue
    {
        private RawData _mainRec;
        private IMasterDataStore _masterStore;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _masterStore = colValRulesParams.MasterStore;
            _mainRec = colValRulesParams.MainRec;
        }

        public string CalculateColValue()
        {
            return LookupFunctions.LookupCurrencySymbol(_mainRec.Moneytype, _masterStore);
        }
    }
}
