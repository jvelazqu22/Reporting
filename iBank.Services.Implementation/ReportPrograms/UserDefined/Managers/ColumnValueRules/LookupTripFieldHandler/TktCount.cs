﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class TktCount : IColumnValue
    {
        private RawData _mainRec;
        private int _tripSummaryLevel;
        private UserReportColumnInformation _column;
        private UserDefinedParameters _userDefinedParams;
        private List<Tuple<string, string>> TripSummaryLevel { get; set; }
        private ShareLogic _sharedLogic;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _tripSummaryLevel = colValRulesParams.TripSummaryLevelInt;
            _column = colValRulesParams.Column;
            _userDefinedParams = colValRulesParams.UserDefinedParams;
            TripSummaryLevel = colValRulesParams.TripSummaryLevelTuple;
            _sharedLogic = new ShareLogic(colValRulesParams);
        }

        public string CalculateColValue()
        {
            if (_tripSummaryLevel == 1)
            {
                return _sharedLogic.CalcTripSummaryField(_mainRec.Recloc, _column.Name) 
                    ? _userDefinedParams.TripDataList.Count(s => s.Recloc.Equals(_mainRec.Recloc) && !string.IsNullOrEmpty(s.Ticket.Trim())).ToString() 
                    : string.Empty;
            }
            return _mainRec.Trantype.EqualsIgnoreCase("I") && !string.IsNullOrEmpty(_mainRec.Ticket.Trim()) ? "00000001" : "00000000";
        }
    }
}