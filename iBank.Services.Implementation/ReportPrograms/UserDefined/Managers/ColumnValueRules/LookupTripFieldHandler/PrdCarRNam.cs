using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class PrdCarRNam : IColumnValue
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
            var aline = LookupFunctions.LookupAirline(_masterStore, _mainRec.Predomcarr);
            return aline.Item2;
        }
    }
}
