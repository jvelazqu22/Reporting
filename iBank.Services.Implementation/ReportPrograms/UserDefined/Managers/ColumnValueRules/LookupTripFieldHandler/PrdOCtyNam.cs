﻿using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class PrdOCtyNam : IColumnValue
    {
        private RawData _mainRec;
        private IMasterDataStore _masterStore;
        private bool _isTitleCaseRequired;
        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _masterStore = colValRulesParams.MasterStore;
            _isTitleCaseRequired = colValRulesParams.IsTitleCaseRequired;
        }

        public string CalculateColValue()
        {
            return LookupFunctions.LookupAportCity(_mainRec.TrpPrdOrig, _masterStore, _isTitleCaseRequired, "A");
        }
    }
}
