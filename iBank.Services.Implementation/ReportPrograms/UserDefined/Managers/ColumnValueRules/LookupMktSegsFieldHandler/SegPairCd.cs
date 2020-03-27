using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupMktSegsFieldHandler
{
    public class SegPairCd : IColumnValue
    {
        private MarketSegmentRawData _marketSegmentRawData;
        private ReportGlobals _globals;
        private IMasterDataStore _masterStore;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _marketSegmentRawData = colValRulesParams.MarketSegmentRawData;
            _globals = colValRulesParams.Globals;
            _masterStore = colValRulesParams.MasterStore;
        }

        public string CalculateColValue()
        {
            //GSA - Seg level City Pair Code Domestic use Airport, Intl use Metro
            if (_globals.Agency.Equals("GSA"))
            {
                return LookupFunctions.LookUpCityPairCode_GSA(_marketSegmentRawData.Mktsegboth, _marketSegmentRawData.DitCode, _masterStore);
            }
            else
            {
                return LookupFunctions.LookUpCityPairCode(_marketSegmentRawData.Mktsegboth, _marketSegmentRawData.DitCode, _masterStore);
            }
        }
    }
}
