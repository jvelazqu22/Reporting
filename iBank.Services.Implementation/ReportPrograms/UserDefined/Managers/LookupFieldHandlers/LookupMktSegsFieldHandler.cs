using System;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers
{
    public class LookupMktSegsFieldHandler
    {
        private readonly UserDefinedParameters _userDefinedParams;
        private readonly IMasterDataStore _masterStore;
        private readonly bool _isTitleCaseRequired;
        private readonly ReportGlobals _globals;

        public SegmentOrLeg _segmentOrLeg;

        public LookupMktSegsFieldHandler(UserDefinedParameters userDefinedParams, IMasterDataStore masterStore, bool isTitleCaseRequired, ReportGlobals globals, SegmentOrLeg segmentOrLeg)
        {
            _userDefinedParams = userDefinedParams;
            _masterStore = masterStore;
            _isTitleCaseRequired = isTitleCaseRequired;
            _globals = globals;
            _segmentOrLeg = segmentOrLeg;
        }

        public string HandleLookupFieldMktSegs(UserReportColumnInformation column, RawData mainRec, int segnum, ColumnValueRulesFactory factory)
        {
            if (!Features.HandleLookupFieldMktSegsRefactor.IsEnabled()) return HandleLookupFieldMktSegsLegacy(column, mainRec, segnum);

            var rec = _userDefinedParams.MarketSegmentLookup[mainRec.RecKey].FirstOrDefault(x => x.Segnum == segnum + 1);

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
        private ColValRulesParams GetColValRulesParams(UserReportColumnInformation column, RawData mainRec, MarketSegmentRawData marketSegmentRawData)
        {
            var colValRulesParams = new ColValRulesParams()
            {
                MasterStore = _masterStore,
                Globals = _globals,
                MainRec = mainRec,
                IsTitleCaseRequired = _isTitleCaseRequired,
                Column = column,
                UserDefinedParams = _userDefinedParams,
                MarketSegmentRawData = marketSegmentRawData
            };
            if (_globals.ParmHasValue(WhereCriteria.RBAPPLYTOLEGORSEG))
            {
                colValRulesParams.SegmentOrLeg = _globals.ParmValueEquals(WhereCriteria.RBAPPLYTOLEGORSEG, "1")
                    ? SegmentOrLeg.Leg
                    : SegmentOrLeg.Segment;
            }
            return colValRulesParams;
        }

        public string HandleLookupFieldMktSegsLegacy(UserReportColumnInformation column, RawData mainRec, int segnum)
        {
            var rec = _userDefinedParams.MarketSegmentLookup[mainRec.RecKey].FirstOrDefault(x => x.Segnum == segnum + 1);

            if (rec == null) return string.Empty;

            switch (column.Name)
            {
                case "GROSAVGCPM": // class
                    //cbuf2 = "(iif(curMktsegs.miles != 0, abs(round(curMktsegs.grossamt/curMktsegs.miles,2)), 0.00))"
                    return rec.Miles != 0 && rec.Grossamt.HasValue ? Math.Abs(MathHelper.Round(rec.Grossamt.Value / rec.Miles, 2)).ToString(CultureInfo.InvariantCulture) : "0";
                case "ODOTRMETRO": // class
                    //cbuf2 = "luMetroCode(curMktsegs.segdest,'A')"
                    return LookupFunctions.LookupMetroCode(_masterStore, rec.Segdest, "A");
                case "ODOTRCTRY": // class
                    //cbuf2 = "luCtryNbr(luAirportCtryCode(curMktsegs.segdest,'A'))"
                    return LookupFunctions.LookupCountryNumber(_masterStore, LookupFunctions.LookupAirportCountryCode(_masterStore, rec.Segdest, "A"), _globals);
                case "MKTDSTATE": // class
                    //cbuf2 = "luStateName(luAirportState(curMktsegs.segdest,'A'))"
                    return LookupFunctions.LookupStateName(LookupFunctions.LookupAirportState(_masterStore, rec.Segdest, "A"), _masterStore);
                case "MKTDCNAMS": // class
                    //cbuf2 = "luCityPair(curMktsegs.mktseg,'A','CITY')"
                    return LookupFunctions.LookupCityPair(rec.Mktseg, "A", "CITY", _masterStore, _isTitleCaseRequired);
                case "MKTDANAMS": // class
                    //cbuf2 = "luCityPair(curMktsegs.mktseg,'A','NAME')"
                    return LookupFunctions.LookupCityPair(rec.Mktseg, "A", "NAME", _masterStore, _isTitleCaseRequired);
                case "MKTNDCNAMS": // class
                    //cbuf2 = "luCityPair(curMktsegs.mktsegboth,'A','CITY')"
                    return LookupFunctions.LookupCityPair(rec.Mktsegboth, "A", "CITY", _masterStore, _isTitleCaseRequired);
                case "MKNDMETRO":
                    //cbuf2 = "luCityPair(curMktsegs.mktsegboth,'A','METRO')"
                    return LookupFunctions.LookupCityPair(rec.Mktsegboth, "A", "METRO", _masterStore, _isTitleCaseRequired);
                case "MKNDANAMS": // class
                    //cbuf2 = "luCityPair(curMktsegs.mktsegboth,'A','NAME')"
                    return LookupFunctions.LookupCityPair(rec.Mktsegboth, "A", "NAME", _masterStore, _isTitleCaseRequired);
                case "MFLTNO": // class
                    //cbuf2 = "curMktsegs.fltno"
                    return rec.Fltno;
                case "MDITCODE": // class
                    //cbuf2 = "curMktsegs.ditcode"
                    return rec.DitCode;
                case "MAIRLINE": // class
                    //cbuf2 = "curMktsegs.airline"
                    return rec.Airline;
                case "MKDURATION": //class
                    var defaultDate = new DateTime(1900, 1, 1, 0, 0, 0);
                    if (_globals.Agency.Equals("GSA"))
                    {
                        if (mainRec.DepDate == null || mainRec.Arrdate == null) return string.Empty;
                        if (defaultDate == mainRec.DepDate || defaultDate == mainRec.Arrdate) return string.Empty;
                    }
                    //MKDURATION = (h)ibMktSegs.flduration
                    return rec.Flduration.ToString();
                case "BICITYPAIR": // class
                    //TODO code may need to change - new lookup function for Bi Directional O&D Code 
                    return rec.Mktsegboth;
                case "BIPAIRDES": // class
                    //TODO code may need to change - new lookup function for Bi Directional O&D Code 
                    if (_globals.Agency.Equals("GSA"))
                    {
                        return LookupFunctions.LookupCityPair_GSA(rec.Mktsegboth, "A", "CITYDESC", _masterStore, _isTitleCaseRequired);
                    }
                    else
                    {
                        return LookupFunctions.LookupCityPair(rec.Mktsegboth, "A", "CITYDESC", _masterStore, _isTitleCaseRequired);
                    }
                case "SEGPAIRCD": // class //GSA - Seg level City Pair Code Domestic use Airport, Intl use Metro
                    if (_globals.Agency.Equals("GSA"))
                    {
                        return LookupFunctions.LookUpCityPairCode_GSA(rec.Mktsegboth, rec.DitCode, _masterStore);
                    }
                    else
                    {
                        return LookupFunctions.LookUpCityPairCode(rec.Mktsegboth, rec.DitCode, _masterStore);
                    }
                case "SEGPRDESC": // class
                    //Uses City Pair Code.  Descriptive Name.  
                    //For domestic trips use City Name + Airport Code, for international trips use City Name only. 
                    if (_globals.Agency.Equals("GSA"))
                    {
                        return GetGSASegmentCityPairDescription(rec.Mktsegboth, rec.DitCode, _masterStore);
                    }
                    else
                    {
                        return LookupFunctions.LookupCityPairDescription(rec.Mktsegboth, rec.DitCode, _masterStore, _isTitleCaseRequired);
                    }
                default:
                    return ReportBuilder.GetValueAsString(rec, column.Name);
            }
        }

        private string GetGSASegmentCityPairDescription(string cityPair, string domintl, IMasterDataStore store)
        {
            var cityPairGSA = LookupFunctions.LookUpCityPairCode_GSA(cityPair, domintl, store);
            if (cityPairGSA == "") return "";
            return GetGSACityPairDescription(cityPairGSA, domintl, store);
        }

        private string GetGSACityPairDescription(string cityPair, string domintl, IMasterDataStore store)
        {
            var splitValues = cityPair.Trim().Split('-');
            var origin = (splitValues[0].CompareTo(splitValues[1]) < 0) ? splitValues[0] : splitValues[1];
            var destination = (splitValues[0].CompareTo(splitValues[1]) < 0) ? splitValues[1] : splitValues[0];

            var originName = LookupFunctions.LookupAportCity(origin, store, true, "A").Trim();
            var originCountryCode = LookupFunctions.LookupCountryCode(origin, store);
            var destinationCountryCode = LookupFunctions.LookupCountryCode(destination, store);
            var appendOriginDestToOriginDestName = (domintl.Trim() == "D" ||
                                                    (originCountryCode.EqualsIgnoreCase("US") &&
                                                     destinationCountryCode.EqualsIgnoreCase("US")));

            if (appendOriginDestToOriginDestName) originName += " (" + origin.Trim() + ")";

            var destinationName = LookupFunctions.LookupAportCity(destination, store, true, "A").Trim();

            if (appendOriginDestToOriginDestName) destinationName += " (" + destination.Trim() + ")";

            return string.Format($"{originName}-{destinationName}");
        }

    }
}
