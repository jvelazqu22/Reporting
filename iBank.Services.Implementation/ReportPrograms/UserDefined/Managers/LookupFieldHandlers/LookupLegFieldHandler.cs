using System;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using CODE.Framework.Core.Utilities.Extensions;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ReportLokkup;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;
using iBank.Services.Implementation.Utilities;
using MoreLinq;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers
{
    public class LookupLegFieldHandler
    {
        private readonly UserDefinedParameters _userDefinedParams;
        private readonly IMasterDataStore _masterStore;
        private readonly IClientDataStore _clientStore;
        private readonly bool _isTitleCaseRequired;
        private readonly ReportGlobals _globals;
        private readonly bool _isDdTime;
        private ReportLookups _reportLookups;

        public SegmentOrLeg _segmentOrLeg;

        public  LookupLegFieldHandler(UserDefinedParameters userDefinedParams, IMasterDataStore masterStore, IClientDataStore clientStore, 
            bool isTitleCaseRequired, ReportGlobals globals, bool isDdTime, ReportLookups reportLookups, SegmentOrLeg segmentOrLeg)
        {
            _userDefinedParams = userDefinedParams;
            _masterStore = masterStore;
            _clientStore = clientStore;
            _isTitleCaseRequired = isTitleCaseRequired;
            _globals = globals;
            _isDdTime = isDdTime;
            _reportLookups = reportLookups;
            _segmentOrLeg = segmentOrLeg;
        }

        public string HandleLookupFieldLeg(UserReportColumnInformation column, RawData mainRec, int seqNo, ColumnValueRulesFactory factory)
        {
            if (!Features.HandleLookupFieldLegRefactor.IsEnabled()) return HandleLookupFieldLegLegacy(column, mainRec, seqNo);

            //It appears that for legs, sequence numbers start at 1, not zero. has to use Seq_Cntr because of collapse
            seqNo++;
            var rec = _userDefinedParams.LegLookup[mainRec.RecKey].FirstOrDefault(x => (_segmentOrLeg == SegmentOrLeg.Segment && x.Seg_Cntr == seqNo) || (_segmentOrLeg == SegmentOrLeg.Leg && x.SeqNo == seqNo));
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

        private ColValRulesParams GetColValRulesParams(UserReportColumnInformation column, RawData mainRec, LegRawData legRawData)
        {
            var colValRulesParams = new ColValRulesParams()
            {
                ClientDataStore = _clientStore,
                MasterStore = _masterStore,
                Globals = _globals,
                MainRec = mainRec,
                ReportLookups = _reportLookups,
                IsTitleCaseRequired = _isTitleCaseRequired,
                Column = column,
                UserDefinedParams = _userDefinedParams,
                LegRawData = legRawData,
                IsDdTime = _isDdTime,
            };
            if (_globals.ParmHasValue(WhereCriteria.RBAPPLYTOLEGORSEG))
            {
                colValRulesParams.SegmentOrLeg = _globals.ParmValueEquals(WhereCriteria.RBAPPLYTOLEGORSEG, "1")
                    ? SegmentOrLeg.Leg
                    : SegmentOrLeg.Segment;
            }
            return colValRulesParams;
        }

        public string HandleLookupFieldLegLegacy(UserReportColumnInformation column, RawData mainRec, int seqNo)
        {
            //It appears that for legs, sequence numbers start at 1, not zero. has to use Seq_Cntr because of collapse
            seqNo++;

            var rec = _userDefinedParams.LegLookup[mainRec.RecKey].FirstOrDefault(x => (_segmentOrLeg == SegmentOrLeg.Segment && x.Seg_Cntr == seqNo) || (_segmentOrLeg == SegmentOrLeg.Leg && x.SeqNo == seqNo));

            if (rec == null) return string.Empty;

            switch (column.Name)
            {
                case "ACTFARE": //class
                    return $"{(MathHelper.Round(rec.ActFare + rec.MiscAmt, 2)):0.00}";
                case "AIRCARRIER": //class
                    return LookupFunctions.LookupAline(_masterStore, rec.Airline, isTitleCase: _isTitleCaseRequired);
                case "AIRLNIATA": // class
                    return LookupFunctions.LookupAlineNbr(_masterStore, rec.Airline);
                case "ARRDTDOW": //class
                    return mainRec.Arrdate?.DayOfWeek.ToString().Substring(0, 3) ?? "";
                case "CLASS": //class
                    return rec.ClassCode;
                case "CLASSCDNAM": // class
                case "CLASSCTNAM": // class
                    return _reportLookups.LookupClassCategoryDescription(rec.ClassCat, mainRec.Agency, _clientStore.ClientQueryDb, _isTitleCaseRequired);
                case "CONNECT": //class
                    return rec.Connect.EqualsIgnoreCase("X") ? "Y" : "N";
                case "DESTAIRPT": //class
                    return AportLookup.LookupAport(_masterStore, rec.Destinat, rec.Mode, _globals.Agency);
                case "DSTCOUNTRY": // class
                    return LookupFunctions.LookupCountry(_masterStore, rec.Destinat, _isTitleCaseRequired);
                case "DSTMETCITY": //class
                    return LookupFunctions.LookupAirMetro(_masterStore, rec.Destinat, "C", _isTitleCaseRequired);
                case "DSTMETSTAT": // class
                    return LookupFunctions.LookupAirMetro(_masterStore, rec.Destinat, "S");
                case "DSTREGION": // class
                    return LookupFunctions.LookupRegionCode(rec.Destinat, string.Empty, _masterStore);
                case "FRMISOCTRY": // class
                    return LookupFunctions.LookupCountryNumber(_masterStore, LookupFunctions.LookupCountryCode(rec.Origin, _masterStore));
                case "MILES": // class
                    return (Math.Abs(rec.Miles) * mainRec.PlusMin).ToString();
                case "ORGAIRPT": // class
                    return AportLookup.LookupAport(_masterStore, rec.Origin, rec.Mode, _globals.Agency);
                case "ORGCOUNTRY": //class
                    return LookupFunctions.LookupCountry(_masterStore, rec.Origin, _isTitleCaseRequired);
                case "ORGMETCITY": // class
                    return LookupFunctions.LookupAirMetro(_masterStore, rec.Origin, "C", _isTitleCaseRequired, rec.Mode);
                case "ORGMETSTAT": // class
                    return LookupFunctions.LookupAirMetro(_masterStore, rec.Origin, "S");
                case "ORGREGION": // class
                    return LookupFunctions.LookupRegionCode(rec.Origin, string.Empty, _masterStore);
                case "RARRDTTIM": // class
                    return rec.RArrDate.GetValueOrDefault().MakeDateTime(rec.ArrTime).ToString(CultureInfo.InvariantCulture);
                case "RDEPDTDOW": // class
                    return rec.RDepDate.GetValueOrDefault().DayOfWeek.ToString().Substring(0, 3);
                case "RDEPDTTIM":
                    return rec.RDepDate.GetValueOrDefault().MakeDateTime(rec.DepTime).ToString(CultureInfo.InvariantCulture);
                case "SEGCNTR": // class
                    return "000";
                case "TOISOCTRY": // class
                    return LookupFunctions.LookupCountryNumber(_masterStore, LookupFunctions.LookupCountryCode(rec.Destinat, _masterStore));
                case "SEGCOST": // class
                    return $"{(rec.ActFare + rec.MiscAmt):0.00}";
                case "CPM": // class
                    return (rec.Miles == 0) ? "0.00" : (mainRec.PlusMin * (rec.ActFare + rec.MiscAmt) / rec.Miles).ToString();
                case "ARRTIME": // class
                    return _isDdTime ? SharedProcedures.ConvertTime(rec.ArrTime) : rec.ArrTime;
                case "DEPTIME": // class
                    return _isDdTime ? SharedProcedures.ConvertTime(rec.DepTime) : rec.DepTime;
                case "EQUIPDESC": // class
                    return LookupFunctions.LookupEquipment(rec.Equip, _masterStore);
                //TODO: GSA 
                case "LGBASEFARE": // class
                    //Base Fare Breakout of the Segment Before Taxes
                    return LookupHelpers.GetBaseFareBreakout(rec.RecKey, rec.Basefare, _userDefinedParams.LegDataList);
                case "LBSFARETAX": // class
                    return LookupHelpers.GetTaxesBreakoutByLegs(rec.RecKey, _userDefinedParams.TripDataList, _userDefinedParams.LegDataList);
                case "LEGS_SEGPR": // class
                    return _segmentOrLeg == SegmentOrLeg.Segment
                        ? rec.Farebase?.Trim()
                        : LookupHelpers.GetPredominantFareBasisByHighMiles(mainRec.RecKey, _userDefinedParams.LegDataList);
                case "PRECLSDESC": //class
                    var maxMileageCategory = _userDefinedParams.LegLookup[mainRec.RecKey].MaxBy(x => x.Miles).ClassCat;
                    var catclass = _reportLookups.LookupClassCategoryDescription(maxMileageCategory, mainRec.Agency, _clientStore.ClientQueryDb, _isTitleCaseRequired);
                    //GSA want this to be UPPERCASE
                    return _globals.Agency == "GSA" ? catclass.Upper() : catclass;

                case "DSTMETCODE": // class
                    return LookupFunctions.LookupAirMetroCode(_masterStore, rec.Destinat, rec.Mode);
                case "ORGMETCODE": // class
                    return LookupFunctions.LookupAirMetroCode(_masterStore, rec.Origin, rec.Mode);
                case "ORGCONTINT": // class
                    return LookupFunctions.LookupRegionName(rec.Origin, _masterStore, _isTitleCaseRequired);
                case "DSTCONTINT": // class
                    return LookupFunctions.LookupRegionName(rec.Destinat, _masterStore, _isTitleCaseRequired);
                case "ORGDSTCNTY": // class
                    return LookupFunctions.LookupOrgCountryToDestCountry(rec.Origin, rec.Destinat, _isTitleCaseRequired, _masterStore);
                case "ORGDSTCONT": // class
                    return LookupFunctions.LookupOrgContinentToDestContinent(rec.Origin, rec.Destinat, _isTitleCaseRequired, _masterStore);
                case "LGFARETYPE": // class
                    if (Features.FareType.IsEnabled())
                    {
                        if (rec.Farebase == null) return "Other";
                        return FareTypeHandler.LookupFareType(rec.Farebase.Trim());
                    }
                    else
                    {
                        return rec.Farebase != null
                        ? _reportLookups.LookupFareType(rec.Farebase.Trim())
                        : "Other";
                    }
                case "DSTCTRYCOD": // class
                    return LookupFunctions.LookupCountryCode(rec.Destinat, _masterStore);
                case "FLDURATION": // class
                    var defaultDate = new DateTime(1900, 1, 1, 0, 0, 0);
                    if (_globals.Agency.Equals("GSA"))
                    {
                        if (mainRec.DepDate == null || mainRec.Arrdate == null) return string.Empty;
                        if (defaultDate == mainRec.DepDate || defaultDate == mainRec.Arrdate) return string.Empty;
                    }
                    return ReportBuilder.GetValueAsString(rec, column.Name);
                default:
                    return ReportBuilder.GetValueAsString(rec, column.Name);
            }
        }
    }
}
