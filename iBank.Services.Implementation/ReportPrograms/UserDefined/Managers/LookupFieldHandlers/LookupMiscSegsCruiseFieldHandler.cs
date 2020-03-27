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
    public class LookupMiscSegsCruiseFieldHandler
    {
        private readonly UserDefinedParameters _userDefinedParams;
        private readonly IMasterDataStore _masterStore;
        private readonly bool _isTitleCaseRequired;
        public List<Tuple<string, string>> TripSummaryLevel { get; set; }
        private readonly BuildWhere _buildWhere;

        public SegmentOrLeg _segmentOrLeg;

        public LookupMiscSegsCruiseFieldHandler(UserDefinedParameters userDefinedParams, IMasterDataStore masterStore, bool isTitleCaseRequired, SegmentOrLeg segmentOrLeg)
        {
            _userDefinedParams = userDefinedParams;
            _masterStore = masterStore;
            _segmentOrLeg = segmentOrLeg;
            TripSummaryLevel = new List<Tuple<string, string>>();
            _isTitleCaseRequired = isTitleCaseRequired;
        }

        public string HandleLookupFieldMiscSegsCruise(UserReportColumnInformation column, RawData mainRec, int seqNo)
        {
            var rec = _userDefinedParams.MiscSegCruiseLookup[mainRec.RecKey].FirstOrDefault(x => x.Msseqno == seqNo + 1);

            if (rec == null) return string.Empty;

            switch (column.Name)
            {
                case "SEAREGION":
                    //cbuf2 = "luRegionName(curSeasegs.regionid)"
                    return LookupFunctions.LookupRegionName(rec.Regionid, _masterStore, _isTitleCaseRequired);
                case "SEAVENDNAM":
                    //cbuf2 = "curSeasegs.mxvendname"
                    return rec.Mxvendname;
                case "SEASGSTAT":
                    //cbuf2 = "curSeasegs.mxsgstatus"
                    return rec.Mxsgstatus;
                case "SEABASPR1":
                    //cbuf2 = "curSeasegs.baseprice1"
                    return rec.Baseprice1.ToString(CultureInfo.InvariantCulture);
                case "SEABASPR2":
                    //cbuf2 = "curSeasegs.baseprice2"
                    return rec.Baseprice2;
                case "SEAPGMID":
                    //cbuf2 = "curSeasegs.pgmid"
                    return rec.Pgmid;
                case "SEAROOMS":
                    //cbuf2 = "curSeasegs.nbrrooms"
                    return rec.Nbrrooms.ToString();
                case "SEASPCLINF":
                    //cbuf2 = "curSeasegs.spclinfo"
                    return rec.Spclinfo;
                case "SEACOMMSN":
                    //cbuf2 = "curSeasegs.mscommisn"
                    return rec.Mscommisn.ToString(CultureInfo.InvariantCulture);
                case "SEAREGNID":
                    //cbuf2 = "curSeasegs.regionid"
                    return rec.Regionid;
                case "SEASEGNUM":
                    //cbuf2 = "curSeasegs.mssegnum"
                    return rec.Mssegnum.ToString();
                case "SEATOTAMT":
                    //cbuf2 = "curSeasegs.segamt"
                    return rec.Segamt.ToString(CultureInfo.InvariantCulture);
                case "SEADURATN":
                    //cbuf2 = "curSeasegs.msduration"
                    return rec.Msduration.ToString();
                case "SEASHIPNAM":
                    //cbuf2 = "curSeasegs.shipname"
                    return rec.Shipname;
                case "SEACABNTYP":
                    //cbuf2 = "curSeasegs.cabintype"
                    return rec.Cabintype;
                case "SEACABNCAT":
                    //cbuf2 = "curSeasegs.cabincateg"
                    return rec.Cabincateg;
                case "SEACABNLOC":
                    //cbuf2 = "curSeasegs.cabindeck"
                    return rec.Cabindeck;
                case "SEACABNNBR":
                    //cbuf2 = "curSeasegs.cabinnbr"
                    return rec.Cabinnbr;
                case "SEAVENDCOD":
                    //cbuf2 = "curSeasegs.vendorcode"
                    return rec.Vendorcode;
                default:
                    return ReportBuilder.GetValueAsString(rec, column.Name);
            }
        }
    }
}
