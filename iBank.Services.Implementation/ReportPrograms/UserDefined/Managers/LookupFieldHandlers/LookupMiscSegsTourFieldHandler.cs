using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers
{
    public class LookupMiscSegsTourFieldHandler
    {
        private readonly UserDefinedParameters _userDefinedParams;
        private readonly IMasterDataStore _masterStore;
        private readonly bool _isTitleCaseRequired;
        public List<Tuple<string, string>> TripSummaryLevel { get; set; }
        private readonly BuildWhere _buildWhere;

        public SegmentOrLeg _segmentOrLeg;

        public LookupMiscSegsTourFieldHandler(UserDefinedParameters userDefinedParams, IMasterDataStore masterStore, bool isTitleCaseRequired, SegmentOrLeg segmentOrLeg)
        {
            _userDefinedParams = userDefinedParams;
            _masterStore = masterStore;
            _segmentOrLeg = segmentOrLeg;
            TripSummaryLevel = new List<Tuple<string, string>>();
            _isTitleCaseRequired = isTitleCaseRequired;
        }

        public string HandleLookupFieldMiscSegsTour(UserReportColumnInformation column, RawData mainRec, int seqNo)
        {
            var rec = _userDefinedParams.MiscSegTourLookup[mainRec.RecKey].FirstOrDefault(x => x.Msseqno == seqNo);

            if (rec == null) return string.Empty;

            switch (column.Name)
            {
                case "TURREGION":
                    //cbuf2 = "luRegionName(curTursegs.regionid)"
                    return LookupFunctions.LookupRegionName(rec.Regionid, _masterStore, _isTitleCaseRequired);
                case "TURVENDNAM":
                    //cbuf2 = "curTursegs.mxvendname"
                    return rec.Mxvendname;
                case "TURSGSTAT":
                    //cbuf2 = "curTursegs.mxsgstatus"
                    return rec.Mxsgstatus;
                case "TURDESTNAM":
                    //cbuf2 = "curTursegs.msdestinat"
                    return rec.Msdestinat;
                case "TURBASPR1":
                    //cbuf2 = "curTursegs.baseprice1"
                    return rec.Baseprice1.ToString(CultureInfo.InvariantCulture);
                case "TURBASPR2":
                    //cbuf2 = "curTursegs.baseprice2"
                    return rec.Baseprice2;
                case "TURPGMID":
                    //cbuf2 = "curTursegs.pgmid"
                    return rec.Pgmid;
                case "TURROOMS":
                    //cbuf2 = "curTursegs.nbrrooms"
                    return rec.Nbrrooms.ToString();
                case "TURBKNGS":
                    //cbuf2 = "curTursegs.msplusmin"
                    return rec.Msplusmin.ToString();
                case "TURSPCLINF":
                    //cbuf2 = "curTursegs.spclinfo"
                    return rec.Spclinfo;
                case "TURARRDATE":
                    //cbuf2 = "curTursegs.msarrdate"
                    return rec.Msarrdate.GetValueOrDefault().ToShortDateString();
                case "TURCOMMSN":
                    //cbuf2 = "curTursegs.mscommisn"
                    return rec.Mscommisn.ToString();
                case "TURORIGNAM":
                    //cbuf2 = "curTursegs.msorigin"
                    return rec.Msorigin;
                case "TURREGNID":
                    //cbuf2 = "curTursegs.regionid"
                    return rec.Regionid;
                case "TURDEST":
                    //cbuf2 = "curTursegs.msdestcode"
                    return rec.Msdestcode;
                case "TURSEGNUM":
                    //cbuf2 = "curTursegs.mssegnum"
                    return rec.Mssegnum.ToString();
                case "TURTOTAMT":
                    //cbuf2 = "curTursegs.segamt"
                    return rec.Segamt.ToString(CultureInfo.InvariantCulture);
                case "TURDURATN":
                    //cbuf2 = "curTursegs.msduration"
                    return rec.Msduration.ToString();
                case "TURORGCODE":
                    //cbuf2 = "curTursegs.msorgcode"
                    return rec.Msorgcode;

                default:
                    return ReportBuilder.GetValueAsString(rec, column.Name);
            }
        }
    }
}
