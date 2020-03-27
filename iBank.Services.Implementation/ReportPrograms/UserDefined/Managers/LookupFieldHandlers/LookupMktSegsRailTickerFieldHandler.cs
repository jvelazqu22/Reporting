using System;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers
{
    public class LookupMktSegsRailTickerFieldHandler
    {
        private readonly UserDefinedParameters _userDefinedParams;
        public SegmentOrLeg _segmentOrLeg;
        private readonly ReportGlobals _globals;

        public LookupMktSegsRailTickerFieldHandler(UserDefinedParameters userDefinedParams, SegmentOrLeg segmentOrLeg, ReportGlobals globals)
        {
            _userDefinedParams = userDefinedParams;
            _segmentOrLeg = segmentOrLeg;
            _globals = globals;
        }

        public string HandleLookupFieldMiscSegsRailTicket(UserReportColumnInformation column, RawData mainRec, int seqNo, ColumnValueRulesFactory factory)
        {
            if (!Features.HandleLookupFieldMiscSegsRailTicketRefactor.IsEnabled()) return HandleLookupFieldMiscSegsRailTicketLegacy(column, mainRec, seqNo);

            var rec = _userDefinedParams.MiscSegRailTicketLookup[mainRec.RecKey].FirstOrDefault(x => x.Msseqno == seqNo);
            if (rec == null) return string.Empty;

            try
            {
                var colValRulesParams = GetColValRulesParams(column, mainRec, rec);
                var columnValue = factory.CreateInstance(column.Name, this);
                columnValue.SetupParams(colValRulesParams);
                return columnValue.CalculateColValue();
            }
            catch (Exception e)
            {
                if (e.InnerException != null) ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                throw;
            }
        }

        private ColValRulesParams GetColValRulesParams(UserReportColumnInformation column, RawData mainRec, MiscSegSharedRawData miscSegSharedRawData)
        {
            var colValRulesParams = new ColValRulesParams()
            {
                Globals = _globals,
                MainRec = mainRec,
                Column = column,
                UserDefinedParams = _userDefinedParams,
                MiscSegSharedRawData = miscSegSharedRawData
            };
            if (_globals.ParmHasValue(WhereCriteria.RBAPPLYTOLEGORSEG))
            {
                colValRulesParams.SegmentOrLeg = _globals.ParmValueEquals(WhereCriteria.RBAPPLYTOLEGORSEG, "1")
                    ? SegmentOrLeg.Leg
                    : SegmentOrLeg.Segment;
            }
            return colValRulesParams;
        }

        public string HandleLookupFieldMiscSegsRailTicketLegacy(UserReportColumnInformation column, RawData mainRec, int seqNo)
        {
            var rec = _userDefinedParams.MiscSegRailTicketLookup[mainRec.RecKey].FirstOrDefault(x => x.Msseqno == seqNo);

            if (rec == null) return string.Empty;

            switch (column.Name)
            {
                case "RALSEGSTAT": // class
                    //cbuf2 = "curRalsegs.mxsgstatus"
                    return rec.Mxsgstatus;
                case "RALDEPDATE": // class
                    //cbuf2 = "curRalsegs.msdepdate"
                    return rec.Msdepdate.GetValueOrDefault().ToString(CultureInfo.InvariantCulture);
                case "RALORIGNAM": // class
                    //cbuf2 = "curRalsegs.msorigin"
                    return rec.Msorigin;
                case "RALDEPTIM": // class
                    //cbuf2 = "curRalsegs.msdeptime"
                    return rec.Msdeptime;
                case "RALARRDATE": // class
                    //cbuf2 = "curRalsegs.msarrdate"
                    return rec.Msarrdate.GetValueOrDefault().ToString(CultureInfo.InvariantCulture);
                case "RALDESTNAM": // class
                    //cbuf2 = "curRalsegs.msdestinat"
                    return rec.Msdestinat;
                case "RALARRTIM": // class
                    //cbuf2 = "curRalsegs.msarrtime"
                    return rec.Msarrtime;
                case "RALDURATN": // class
                    var defaultDate = new DateTime(1900, 1, 1, 0, 0, 0);
                    if (_globals.Agency.Equals("GSA"))
                    {
                        if (mainRec.DepDate == null || mainRec.Arrdate == null) return string.Empty;
                        if (defaultDate == mainRec.DepDate || defaultDate == mainRec.Arrdate) return string.Empty;
                    }
                    //cbuf2 = "curRalsegs.msduration"
                    return rec.Msduration.ToString();
                case "RALVENDNAM": //class
                    //cbuf2 = "curRalsegs.mxvendname"
                    return rec.Mxvendname;
                case "RALVENDCOD": // class
                    //cbuf2 = "curRalsegs.vendorcode"
                    return rec.Vendorcode;
                case "RALCLASCOD": // class
                    //cbuf2 = "curRalsegs.class"
                    return rec.Class;
                case "RALTRNNBR": // class
                    //cbuf2 = "curRalsegs.svcidnbr"
                    return rec.Svcidnbr;
                case "RALMEALS": // class
                    //cbuf2 = "curRalsegs.mealdesc"
                    return rec.Mealdesc;
                case "RALCONFNBR": // class
                    //cbuf2 = "curRalsegs.confirmno"
                    return rec.Confirmno;
                case "RALBASPR1": //class
                    //cbuf2 = "curRalsegs.baseprice1"
                    return rec.Baseprice1.ToString(CultureInfo.InvariantCulture);
                case "RALBASPR2": //class
                    //cbuf2 = "curRalsegs.baseprice2"
                    return rec.Baseprice2;
                case "RALORGSTN":
                    //cbuf2 = "curRalsegs.msorgcode"
                    return rec.Msorgcode;
                case "RALDSTSTN": // class
                    //cbuf2 = "curRalsegs.msdestcode"
                    return rec.Msdestcode;
                case "RALSEGNUM": // class
                    //cbuf2 = "curRalsegs.mssegnum"
                    return rec.Mssegnum.ToString();
                case "RALTOTAMT": //class
                    //cbuf2 = "curRalsegs.segamt"
                    return rec.Segamt.ToString(CultureInfo.InvariantCulture);
                default:
                    return ReportBuilder.GetValueAsString(rec, column.Name);
            }
        }

    }
}
