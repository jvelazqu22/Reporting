using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupMktSegsFieldHandler
{
    public class SegPrDesc : IColumnValue
    {
        private MarketSegmentRawData _marketSegmentRawData;
        private ReportGlobals _globals;
        private IMasterDataStore _masterStore;
        private bool _isTitleCaseRequired;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _marketSegmentRawData = colValRulesParams.MarketSegmentRawData;
            _globals = colValRulesParams.Globals;
            _masterStore = colValRulesParams.MasterStore;
            _isTitleCaseRequired = colValRulesParams.IsTitleCaseRequired;
        }

        public string CalculateColValue()
        {
            //Uses City Pair Code.  Descriptive Name.  
            //For domestic trips use City Name + Airport Code, for international trips use City Name only. 
            if (_globals.Agency.Equals("GSA"))
            {
                return GetGSASegmentCityPairDescription(_marketSegmentRawData.Mktsegboth, _marketSegmentRawData.DitCode, _masterStore);
            }
            else
            {
                return LookupFunctions.LookupCityPairDescription(_marketSegmentRawData.Mktsegboth, _marketSegmentRawData.DitCode, _masterStore, _isTitleCaseRequired);
            }
        }

        private string GetGSASegmentCityPairDescription(string cityPair, string domintl, IMasterDataStore store)
        {
            var cityPairGSA = LookupFunctions.LookUpCityPairCode_GSA(cityPair, domintl, store);
            if (cityPairGSA == "") return "";
            return GetGSACityPairDescription(cityPairGSA, domintl, store);
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
