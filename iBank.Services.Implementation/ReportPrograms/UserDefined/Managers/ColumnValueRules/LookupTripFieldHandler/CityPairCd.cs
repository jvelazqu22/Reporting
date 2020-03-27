using System.Linq;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class CityPairCd : IColumnValue
    {
        private RawData _mainRec;
        private IMasterDataStore _masterStore;
        private UserDefinedParameters _userDefinedParams;
        private ReportGlobals _globals;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _masterStore = colValRulesParams.MasterStore;
            _userDefinedParams = colValRulesParams.UserDefinedParams;
            _globals = colValRulesParams.Globals;
        }

        public string CalculateColValue()
        {
            if (_globals.Agency.Equals("GSA"))
            {
                //return LookupFunctions.LookUpCityPairCode_GSA(mainRec.Trpctypair, mainRec.Domintl, _masterStore);
                //GSA Enhancement 00170668
                return GetGSATripCityPairCodeUseMktSegs(_mainRec.RecKey, _masterStore);
            }
            else
            {
                return LookupFunctions.LookUpCityPairCode(_mainRec.Trpctypair, _mainRec.Domintl, _masterStore);
            }
        }
        private string GetGSATripCityPairCodeUseMktSegs(int reckey, IMasterDataStore dataStore)
        {
            var cityPairSeg = _userDefinedParams.MarketSegmentDataList.FirstOrDefault(x => x.RecKey == reckey && x.Segnum == 1);
            if (cityPairSeg != null)
            {
                return LookupFunctions.LookUpCityPairCode_GSA(cityPairSeg.Mktsegboth, cityPairSeg.DitCode, dataStore);
            }
            return "";
        }

    }
}
