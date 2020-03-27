using CODE.Framework.Core.Utilities.Extensions;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using MoreLinq;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class PreClsDesc : IColumnValue
    {
        private UserDefinedParameters _userDefinedParams;
        private RawData _mainRec;
        private ReportLookups _reportLookups;
        private IClientDataStore _clientStore;
        private bool _isTitleCaseRequired;
        private ReportGlobals _globals;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _userDefinedParams = colValRulesParams.UserDefinedParams;
            _mainRec = colValRulesParams.MainRec;
            _reportLookups = colValRulesParams.ReportLookups;
            _isTitleCaseRequired = colValRulesParams.IsTitleCaseRequired;
            _globals = colValRulesParams.Globals;
            _clientStore = colValRulesParams.ClientDataStore;
        }

        public string CalculateColValue()
        {
            var maxMileageCategory = _userDefinedParams.LegLookup[_mainRec.RecKey].MaxBy(x => x.Miles).ClassCat;
            var catclass = _reportLookups.LookupClassCategoryDescription(maxMileageCategory, _mainRec.Agency, _clientStore.ClientQueryDb, _isTitleCaseRequired);
            //GSA want this to be UPPERCASE
            return _globals.Agency == "GSA" ? catclass.Upper() : catclass;
        }
    }
}
