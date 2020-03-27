using CODE.Framework.Core.Utilities.Extensions;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class TrpClasDsc : IColumnValue
    {
        private RawData _mainRec;
        private ReportLookups _reportLookups;
        private ReportGlobals _globals;
        private IClientDataStore _clientStore;
        private bool _isTitleCaseRequired;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _reportLookups = colValRulesParams.ReportLookups;
            _globals = colValRulesParams.Globals;
            _clientStore = colValRulesParams.ClientDataStore;
            _isTitleCaseRequired = colValRulesParams.IsTitleCaseRequired;
        }

        public string CalculateColValue()
        {
            var catclass = _reportLookups.LookupClassCategoryDescription(LookupFunctions.LookupTripClassCatCode(_reportLookups.Segs, _mainRec.RecKey), 
                _globals.Agency, _clientStore.ClientQueryDb, _isTitleCaseRequired);
            //GSA want this to be UPPERCASE
            return _globals.Agency == "GSA" ? catclass.Upper() : catclass;
        }
    }
}
