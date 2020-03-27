using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class VendNames : IColumnValue
    {
        private RawData _mainRec;
        private int _tripSummaryLevel;
        private UserReportColumnInformation _column;
        private UserDefinedParameters _userDefinedParams;
        private List<Tuple<string, string>> TripSummaryLevel { get; set; }
        private ReportLookups _reportLookups;
        private IClientDataStore _clientStore;
        private ReportGlobals _globals;
        private ShareLogic _sharedLogic;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _tripSummaryLevel = colValRulesParams.TripSummaryLevelInt;
            _column = colValRulesParams.Column;
            _userDefinedParams = colValRulesParams.UserDefinedParams;
            TripSummaryLevel = colValRulesParams.TripSummaryLevelTuple;
            _reportLookups = colValRulesParams.ReportLookups;
            _clientStore = colValRulesParams.ClientDataStore;
            _globals = colValRulesParams.Globals;
            _sharedLogic = new ShareLogic(colValRulesParams);
        }

        public string CalculateColValue()
        {
            if (_tripSummaryLevel == 1)
            {
                if (!_sharedLogic.CalcTripSummaryField(_mainRec.Recloc, _column.Name)) return string.Empty;

                var vendCodes = _userDefinedParams.TripDataList.Where(s => s.Recloc.EqualsIgnoreCase(_mainRec.Recloc) && !string.IsNullOrEmpty(s.Clientid.Trim()) && !string.IsNullOrEmpty(s.Trpvendcod.Trim()));
                var vendDescs = vendCodes.Select(s => _reportLookups.LookupVendorDescription(s.Clientid, s.Trpvendcod, _clientStore.ClientQueryDb, _globals.Agency));
                return string.Join(",", vendDescs);
            }
            return _reportLookups.LookupVendorDescription(_mainRec.Clientid, _mainRec.Trpvendcod, _clientStore.ClientQueryDb, _globals.Agency);
        }
    }
}
