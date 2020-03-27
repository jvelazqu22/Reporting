using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class OrgDetCnty : IColumnValue
    {
        private RawData _mainRec;
        private IMasterDataStore _masterStore;
        private ReportGlobals _globals;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _masterStore = colValRulesParams.MasterStore;
            _globals = colValRulesParams.Globals;
        }

        public string CalculateColValue()
        {
            //Alphabetic:  Derived from Origin Country to Destination Country (all France to United States trips and United States to France trips would show as France-United States)
            return LookupFunctions.LookupCityPairCountryToCountry(_mainRec.Trpctypair, _globals, _masterStore);
        }
    }
}
