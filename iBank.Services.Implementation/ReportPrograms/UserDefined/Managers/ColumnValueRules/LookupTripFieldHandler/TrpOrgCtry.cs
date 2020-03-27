﻿using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class TrpOrgCtry : IColumnValue
    {
        private RawData _mainRec;
        private ReportLookups _reportLookups;
        private IMasterDataStore _masterStore;
        private bool _isTitleCaseRequired;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _reportLookups = colValRulesParams.ReportLookups;
            _masterStore = colValRulesParams.MasterStore;
            _isTitleCaseRequired = colValRulesParams.IsTitleCaseRequired;
        }

        public string CalculateColValue()
        {
            return LookupFunctions.LookupCountry(_masterStore, LookupFunctions.LookupSegRouteFirstOrigin(_reportLookups.Segs, _mainRec.RecKey), _isTitleCaseRequired, _mainRec.Valcarr);
        }
    }
}