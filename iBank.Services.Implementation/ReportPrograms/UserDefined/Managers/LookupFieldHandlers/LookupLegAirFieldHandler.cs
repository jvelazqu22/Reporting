using System;
using System.Globalization;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers
{
    public class LookupLegAirFieldHandler
    {
        private readonly UserDefinedParameters _userDefinedParams;
        private readonly IMasterDataStore _masterStore;
        private readonly IClientDataStore _clientStore;
        private readonly bool _isTitleCaseRequired;
        private readonly ReportGlobals _globals;

        private readonly bool _isDdTime;
        private ReportLookups _reportLookups;
        public SegmentOrLeg _segmentOrLeg;

        public LookupLegAirFieldHandler(UserDefinedParameters userDefinedParams, IMasterDataStore masterStore, IClientDataStore clientStore, bool isTitleCaseRequired,
            ReportGlobals globals, bool isDdTime, ReportLookups reportLookups, SegmentOrLeg segmentOrLeg)
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

        public string HandleLookupFieldLegAir(UserReportColumnInformation column, RawData mainRec, int seqNo)
        {           
            //It appears that for legs, sequence numbers start at 1, not zero. has to use Seq_Cntr because of collapse
            seqNo++;
            //test segentOrLeg, segent use seg_cntr else use segno.
            //need a collpase for userdefined report
            var rec = _userDefinedParams.AirLegLookup[mainRec.RecKey].FirstOrDefault(x => (_segmentOrLeg == SegmentOrLeg.Segment && x.Seg_Cntr == seqNo) || (_segmentOrLeg == SegmentOrLeg.Leg && x.SeqNo == seqNo));

            if (rec == null) return string.Empty;

            switch (column.Name)
            {
                case "AIR_SGCOST":
                    return (rec.ActFare + rec.MiscAmt).ToString();
                case "AIR_CPM":
                    return (rec.Miles == 0) ? "0.00" : (mainRec.PlusMin * (rec.ActFare + rec.MiscAmt) / rec.Miles).ToString();
                case "AIR_MILES":
                    return (Math.Abs(rec.Miles) * mainRec.PlusMin).ToString();
                case "AIR_ACARC2":
                    return rec.AltCarCo2.ToString();
                case "AIR_AIRCO2":
                    return rec.AirCo2.ToString();
                case "AIR_ARALC2":
                    return rec.AltRailCo2.ToString();
                case "AIR_ARRDAT":
                    return rec.RArrDate.GetValueOrDefault().ToShortDateString();
                case "AIR_ARRTIM":
                    return _isDdTime ? SharedProcedures.ConvertTime(rec.ArrTime) : rec.ArrTime;
                case "AIR_CLASS":
                    return rec.ClassCode;
                case "AIR_CLASCT":
                    return rec.ClassCat;
                case "AIR_DEPDAT":
                    return rec.RDepDate.GetValueOrDefault().ToShortDateString();
                case "AIR_DEPTIM":
                    return _isDdTime ? SharedProcedures.ConvertTime(rec.DepTime) : rec.DepTime;
                case "AIR_DITCOD":
                    return rec.DitCode;
                case "AIR_DSTNAT":
                    return rec.Destinat;
                case "AIR_FARBAS":
                    return rec.Farebase;
                case "AIR_FLTNO":
                    return rec.fltno;
                case "AIR_MODE":
                    return rec.Mode;
                case "AIR_MSCAMT":
                    return rec.MiscAmt.ToString();
                case "AIR_AIRLIN":
                    return rec.Airline;
                case "AIR_ORIGIN":
                    return rec.Origin;
                case "AIR_PLSMIN":
                    return rec.Rplusmin.ToString();
                case "AIR_SEAT":
                    return rec.Seat;
                case "AIR_SEQNO":
                case "AIR_SEQSS":
                    return rec.SeqNo.ToString();
                case "AIR_SGSTAT":
                    return rec.Segstatus;
                case "AIR_TKTDES":
                    return rec.Tktdesig;
                case "AIR_ACTFAR":
                    return string.Format("{0:0.00}", (rec.ActFare + rec.MiscAmt));
                case "AIR_DEPDOW":
                    return rec.RDepDate.GetValueOrDefault().DayOfWeek.ToString().Substring(0, 3);
                case "AIR_ARRDOW":
                    return rec.RArrDate.GetValueOrDefault().DayOfWeek.ToString().Substring(0, 3);
                case "AIR_CONNEC":
                    return rec.Connect.EqualsIgnoreCase("X") ? "Y" : "N";
                case "AIR_SEGPRD":
                    return _segmentOrLeg == SegmentOrLeg.Segment
                               ? rec.Farebase?.Trim()
                               : LookupHelpers.GetPredominantFareBasisByHighMiles(mainRec.RecKey, _userDefinedParams.AirLegDataList);
                case "AIR_AIRCAR":
                    return LookupFunctions.LookupAline(_masterStore, rec.Airline, isTitleCase: _isTitleCaseRequired);
                case "AIR_ARDTTM":
                    return rec.RArrDate.GetValueOrDefault().MakeDateTime(rec.ArrTime).ToString(CultureInfo.InvariantCulture);
                case "AIR_ARIATA":
                    return LookupFunctions.LookupAirline(_masterStore, rec.Airline).Item1.ToString("D3");
                case "AIR_CLSNAM":
                    return _reportLookups.LookupClassCategoryDescription(rec.ClassCat, _globals.Agency, _clientStore.ClientQueryDb, _isTitleCaseRequired);
                case "AIR_DAIRPT":
                    return AportLookup.LookupAport(_masterStore, rec.Destinat, rec.Mode, _globals.Agency);
                case "AIR_DMTCTY":
                    return LookupFunctions.LookupAirMetro(_masterStore, rec.Destinat, "C");
                case "AIR_DMTSTT":
                    return LookupFunctions.LookupAirMetro(_masterStore, rec.Destinat, "S");
                case "AIR_DSTRGN":
                    return LookupFunctions.LookupRegionCode(rec.Destinat, string.Empty, _masterStore);
                case "AIR_DTCTRY":
                    return LookupFunctions.LookupCountryCode(rec.Destinat, _masterStore);
                case "AIR_OAIRPT":
                    return AportLookup.LookupAport(_masterStore, rec.Origin, rec.Mode, _globals.Agency);
                case "AIR_OMTSTT":
                    return LookupFunctions.LookupAirMetro(_masterStore, rec.Origin, "S");
                case "AIR_ORCTRY":
                    return LookupFunctions.LookupCountryCode(rec.Origin, _masterStore);
                case "AIR_ORGRGN":
                    return LookupFunctions.LookupRegionCode(rec.Origin, string.Empty, _masterStore);
                case "AIR_ORMCTY":
                    return LookupFunctions.LookupAirMetro(_masterStore, rec.Origin, "C");
                case "AIR_RPDTTM":
                    return rec.RDepDate.GetValueOrDefault().MakeDateTime(rec.DepTime).ToString(CultureInfo.InvariantCulture);
                case "AIR_SGCNTR":
                    return "000";
                default:
                    return ReportBuilder.GetValueAsString(rec, column.Name);
            }
        }
    }
}
