using System;
using System.Collections.Generic;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.Client;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using Domain.Helper;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules
{
    public class ColValRulesParams
    {
        public ClientFunctions ClientFunctions { get; set; }
        public IClientDataStore ClientDataStore { get; set; }
        public IMasterDataStore MasterStore { get; set; }
        public ReportGlobals Globals { get; set; }
        public RawData MainRec { get; set; }
        public ReportLookups ReportLookups { get; set; }
        public bool IsTitleCaseRequired { get; set; }
        public int TripSummaryLevelInt { get; set; }
        public UserReportColumnInformation Column { get; set; }
        public UserDefinedParameters UserDefinedParams { get; set; }
        public List<Tuple<string, string>> TripSummaryLevelTuple { get; set; }
        public BuildWhere BuildWhere { get; set; }
        public SegmentOrLeg SegmentOrLeg { get; set; }
        public LegRawData LegRawData { get; set; }
        public MarketSegmentRawData MarketSegmentRawData { get; set; }
        public bool IsDdTime { get; set; }
        public MiscSegSharedRawData MiscSegSharedRawData { get; set; }
    }
}
