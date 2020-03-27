using System.Linq;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class CityPrDesc : IColumnValue
    {
        private RawData _mainRec;
        private IMasterDataStore _masterStore;
        private UserDefinedParameters _userDefinedParams;
        private ReportGlobals _globals;
        private bool _isTitleCaseRequired;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _masterStore = colValRulesParams.MasterStore;
            _isTitleCaseRequired = colValRulesParams.IsTitleCaseRequired;
            _userDefinedParams = colValRulesParams.UserDefinedParams;
            _globals = colValRulesParams.Globals;
        }

        public string CalculateColValue()
        {
            //Uses City Pair Code.  Descriptive Name.  
            //For domestic trips use City Name + Airport Code, for international trips use City Name only. 

            if (_globals.Agency.Equals("GSA"))
            {
                //return LookupFunctions.LookupCityPairDescription_GSA(mainRec.Trpctypair, mainRec.Domintl, _masterStore, _isTitleCaseRequired);
                //GSA Enhancement 00170668
                return GetGSATripCityPairDescriptionMktSegs(_mainRec.RecKey, _masterStore);
            }
            else
            {
                return LookupFunctions.LookupCityPairDescription(_mainRec.Trpctypair, _mainRec.Domintl, _masterStore, _isTitleCaseRequired);
            }
        }

        private string GetGSATripCityPairDescriptionMktSegs(int reckey, IMasterDataStore store)
        {
            var cityPairSeg = _userDefinedParams.MarketSegmentDataList.FirstOrDefault(x => x.RecKey == reckey && x.Segnum == 1);
            if (cityPairSeg == null) return "";

            var cityPair = LookupFunctions.LookUpCityPairCode_GSA(cityPairSeg.Mktsegboth, cityPairSeg.DitCode, store);

            return GetGSACityPairDescription(cityPair, cityPairSeg.DitCode, store);
        }

        private string GetGSACityPairDescription(string cityPair, string domintl, IMasterDataStore store)
        {
            var splitValues = cityPair.Trim().Split('-');
            var origin = (splitValues[0].CompareTo(splitValues[1]) < 0) ? splitValues[0] : splitValues[1];
            var destination = (splitValues[0].CompareTo(splitValues[1]) < 0) ? splitValues[1] : splitValues[0];

            var originName = LookupFunctions.LookupAportCity(origin, store, true, "A").Trim();
            var originCountryCode = LookupFunctions.LookupCountryCode(origin, store);
            var destinationCountryCode = LookupFunctions.LookupCountryCode(destination, store);
            var appendOriginDestToOriginDestName = (domintl.Trim() == "D" ||
                                                    (originCountryCode.EqualsIgnoreCase("US") &&
                                                     destinationCountryCode.EqualsIgnoreCase("US")));

            if (appendOriginDestToOriginDestName) originName += " (" + origin.Trim() + ")";

            var destinationName = LookupFunctions.LookupAportCity(destination, store, true, "A").Trim();

            if (appendOriginDestToOriginDestName) destinationName += " (" + destination.Trim() + ")";

            return string.Format($"{originName}-{destinationName}");
        }
    }
}
