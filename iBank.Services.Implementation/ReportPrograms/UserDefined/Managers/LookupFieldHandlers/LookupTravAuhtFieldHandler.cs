using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers
{
    public class LookupTravAuhtFieldHandler
    {
        private readonly UserDefinedParameters _userDefinedParams;
        private readonly IClientDataStore _clientStore;
        private readonly ReportGlobals _globals;
        private ReportLookups _reportLookups;
        public List<Tuple<string, string>> TripSummaryLevel { get; set; }
        private readonly BuildWhere _buildWhere;

        public SegmentOrLeg _segmentOrLeg;

        public LookupTravAuhtFieldHandler(UserDefinedParameters userDefinedParams, IClientDataStore clientStore, ReportGlobals globals, 
            SegmentOrLeg segmentOrLeg, ReportLookups reportLookups, BuildWhere buildWhere, List<UserReportColumnInformation> columns)
        {
            _userDefinedParams = userDefinedParams;
            _clientStore = clientStore;
            _globals = globals;
            _segmentOrLeg = segmentOrLeg;
            _reportLookups = reportLookups;
            TripSummaryLevel = new List<Tuple<string, string>>();
            _reportLookups = new ReportLookups(columns, buildWhere, globals);
        }

        public string HandleLookupFieldTravAuth(UserReportColumnInformation column, RawData mainRec)
        {
            var rec = _userDefinedParams.TravAuthLookup[mainRec.RecKey].FirstOrDefault();

            if (rec == null) return string.Empty;

            switch (column.Name)
            {
                case "OUTPOLDESC":
                    //luRsnDescs(t1.outpolcods,t1.acct)
                    return _reportLookups.LookupReason(rec.Outpolcods, _clientStore.ClientQueryDb, _globals.Agency, _globals.UserLanguage);
                case "RTVLDESC":
                    //luRsnDescs(t1.rtvlcode,t1.acct)
                    return _reportLookups.LookupReason(rec.Rtvlcode, _clientStore.ClientQueryDb, _globals.Agency, _globals.UserLanguage);
                case "AUTHSTAT":
                    switch (rec.Authstatus.Trim().ToUpper())
                    {
                        case "P":
                            return "'PENDING'";
                        case "N":
                            return "'NOTIFY'";
                        case "I":
                            return "'IN PROCESS'";
                        case "D":
                            return "'DECLINED'";
                        case "A":
                            return "'APPROVED'";
                        case "E":
                            return "'EXPIRED'";
                        case "C":
                            return "'CANCEL'";
                    }
                    break;
                default:
                    return ReportBuilder.GetValueAsString(rec, column.Name);
            }

            return string.Empty;
        }

    }
}
