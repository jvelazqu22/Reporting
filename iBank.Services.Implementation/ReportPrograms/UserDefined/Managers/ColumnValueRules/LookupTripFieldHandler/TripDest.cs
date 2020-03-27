using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class TripDest : IColumnValue
    {
        private RawData _mainRec;
        private ReportLookups _reportLookups;
        private IMasterDataStore _masterStore;
        private ReportGlobals _globals;


        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _reportLookups = colValRulesParams.ReportLookups;
            _masterStore = colValRulesParams.MasterStore;
            _globals = colValRulesParams.Globals;
        }

        public string CalculateColValue()
        {
            var port = LookupFunctions.LookupSegRouteFirstDestination(_reportLookups.Segs, _mainRec.RecKey);
            return AportLookup.LookupAport(_masterStore, port, _mainRec.Valcarr, _globals.Agency, string.Empty);
        }
    }
}
