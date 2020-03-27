using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupMktSegsFieldHandler
{
    public class OdoTrCtry : IColumnValue
    {
        private MarketSegmentRawData _marketSegmentRawData;
        private IMasterDataStore _masterStore;
        private ReportGlobals _globals;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _marketSegmentRawData = colValRulesParams.MarketSegmentRawData;
            _masterStore = colValRulesParams.MasterStore;
            _globals = colValRulesParams.Globals;
        }

        public string CalculateColValue()
        {
            //cbuf2 = "luCtryNbr(luAirportCtryCode(curMktsegs.segdest,'A'))"
            return LookupFunctions.LookupCountryNumber(_masterStore, LookupFunctions.LookupAirportCountryCode(_masterStore, _marketSegmentRawData.Segdest, "A"), _globals);
        }
    }
}
