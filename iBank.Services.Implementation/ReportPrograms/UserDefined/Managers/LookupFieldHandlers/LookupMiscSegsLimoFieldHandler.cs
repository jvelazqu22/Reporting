using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers
{
    public class LookupMiscSegsLimoFieldHandler
    {
        private readonly UserDefinedParameters _userDefinedParams;
        public List<Tuple<string, string>> TripSummaryLevel { get; set; }
        private readonly BuildWhere _buildWhere;

        public SegmentOrLeg _segmentOrLeg;

        public LookupMiscSegsLimoFieldHandler(UserDefinedParameters userDefinedParams, SegmentOrLeg segmentOrLeg)
        {
            _userDefinedParams = userDefinedParams;
            _segmentOrLeg = segmentOrLeg;
            TripSummaryLevel = new List<Tuple<string, string>>();
        }

        public string HandleLookupFieldMiscSegsLimo(UserReportColumnInformation column, RawData mainRec, int seqNo)
        {
            var rec = _userDefinedParams.MiscSegLimoLookup[mainRec.RecKey].FirstOrDefault(x => x.Msseqno == seqNo);

            if (rec == null) return string.Empty;

            switch (column.Name)
            {
                case "LIMSGSTAT":
                    //cbuf2 = "curLimsegs.mxsgstatus"
                    return rec.Mxsgstatus;
                case "LIMORIGNAM":
                    //cbuf2 = "curLimsegs.msorigin"
                    return rec.Msorigin;
                case "LIMARRDATE":
                    //cbuf2 = "curLimsegs.msarrdate"
                    return rec.Msarrdate.GetValueOrDefault().ToShortDateString();
                case "LIMARRTIM":
                    //cbuf2 = "curLimsegs.msarrtime"
                    return rec.Msarrtime;
                case "LIMDESTNAM":
                    //cbuf2 = "curLimsegs.msdestinat"
                    return rec.Msdestinat;
                case "LIMVENDNAM":
                    //cbuf2 = "curLimsegs.mxvendname"
                    return rec.Mxvendname;
                case "LIMBASPR1":
                    //cbuf2 = "curLimsegs.baseprice1"
                    return rec.Baseprice1.ToString(CultureInfo.InvariantCulture);
                case "LIMBASPR2":
                    //cbuf2 = "curLimsegs.baseprice2"
                    return rec.Baseprice2;
                case "LIMRATTYP":
                    //cbuf2 = "curLimsegs.msratetype"
                    return rec.Msratetype;
                case "LIMCOMMSN":
                    //cbuf2 = "curLimsegs.mscommisn"
                    return rec.Mscommisn.ToString(CultureInfo.InvariantCulture);
                case "LIMTOTAMT":
                    //cbuf2 = "curLimsegs.segamt"
                    return rec.Segamt.ToString(CultureInfo.InvariantCulture);
                case "LIMSEGNUM":
                    //cbuf2 = "curLimsegs.mssegnum"
                    return rec.Mssegnum.ToString();
                default:
                    return ReportBuilder.GetValueAsString(rec, column.Name);
            }
        }
    }
}
