using System;
using System.Globalization;
using System.Linq;
using Domain;
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
    public class LookupLegRailFieldHandler
    {
        private readonly UserDefinedParameters _userDefinedParams;
        private readonly IMasterDataStore _masterStore;
        private readonly IClientDataStore _clientStore;
        private readonly bool _isTitleCaseRequired;
        private readonly ReportGlobals _globals;

        private readonly bool _isDdTime;
        private ReportLookups _reportLookups;

        public SegmentOrLeg _segmentOrLeg;

        public LookupLegRailFieldHandler(UserDefinedParameters userDefinedParams, IMasterDataStore masterStore, IClientDataStore clientStore, bool isTitleCaseRequired,
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

              
        public string HandleLookupFieldLegRail(UserReportColumnInformation column, RawData mainRec, int seqNo)
        {
            //It appears that for legs, sequence numbers start at 1, not zero. 
            seqNo++;
            var rec = _userDefinedParams.RailLegLookup[mainRec.RecKey].FirstOrDefault(s => (_segmentOrLeg == SegmentOrLeg.Segment && s.Seg_Cntr == seqNo) || (_segmentOrLeg == SegmentOrLeg.Leg && s.SeqNo == seqNo));

            if (rec == null) return string.Empty;

            switch (column.Name)
            {
                case "RAL_ACTFAR":
                case "RAL_SGCOST":
                    return (rec.ActFare + rec.MiscAmt).ToString();
                case "RAL_CPM":
                    return (rec.Miles == 0) ? "0.00" : (mainRec.PlusMin * (rec.ActFare + rec.MiscAmt) / rec.Miles).ToString();
                case "RAL_MILES":
                    return (Math.Abs(rec.Miles) * mainRec.PlusMin).ToString(CultureInfo.InvariantCulture);
                case "RAL_ACARC2":
                    return rec.AltCarCo2.ToString();
                case "RAL_AIRCO2":
                    return rec.AirCo2.ToString();
                case "RAL_ARALC2":
                    return rec.AltRailCo2.ToString();
                case "RAL_ARRDAT":
                    return rec.RArrDate.GetValueOrDefault().ToShortDateString();
                case "RAL_ARRTIM":
                    return _isDdTime ? SharedProcedures.ConvertTime(rec.ArrTime) : rec.ArrTime;
                case "RAL_CLASS":
                    return rec.ClassCode;
                case "RAL_CLASCT":
                    return rec.ClassCat;
                case "RAL_DEPDAT":
                    return rec.RDepDate.GetValueOrDefault().ToShortDateString();
                case "RAL_DEPTIM":
                    return _isDdTime ? SharedProcedures.ConvertTime(rec.DepTime) : rec.DepTime;
                case "RAL_DITCOD":
                    return rec.DitCode;
                case "RAL_DSTNAT":
                    //* 06/25/09 - SPECIAL HANDLING: USE origdest WHEN PRESENT. 
                    return !string.IsNullOrEmpty(rec.Origdest) ? rec.Origdest : rec.Destinat;
                case "RAL_FARBAS":
                    return rec.Farebase;
                case "RAL_FLTNO":
                    return rec.fltno;
                case "RAL_MODE":
                    return rec.Mode;
                case "RAL_MSCAMT":
                    return rec.MiscAmt.ToString();
                case "RAL_OPRCOD":
                    //* 06/25/09 - SPECIAL HANDLING: USE origcarr WHEN PRESENT
                    return !string.IsNullOrEmpty(rec.Origcarr) ? rec.Origcarr : rec.Airline;
                case "RAL_ORIGIN":
                    //* 06/25/09 - SPECIAL HANDLING: USE origorigin WHEN PRESENT. 
                    return !string.IsNullOrEmpty(rec.Origorigin) ? rec.Origorigin : rec.Origin;
                case "RAL_PLSMIN":
                    return rec.Rplusmin.ToString();
                case "RAL_SEAT":
                    return rec.Seat;
                case "RAL_SEQNO":
                    return rec.SeqNo.ToString();
                case "RAL_SGSTAT":
                    return rec.Segstatus;
                case "RAL_TKTDES":
                    return rec.Tktdesig;
                case "RAL_ARDTTM":
                    return rec.RArrDate.GetValueOrDefault().MakeDateTime(rec.ArrTime).ToString(CultureInfo.InvariantCulture);
                case "RAL_RPDTTM":
                    return rec.RDepDate.GetValueOrDefault().MakeDateTime(rec.DepTime).ToString(CultureInfo.InvariantCulture);
                case "RAL_ARRDOW":
                    return rec.RArrDate.GetValueOrDefault().DayOfWeek.ToString().Substring(0, 3);
                case "RAL_CLSNAM":
                    return _reportLookups.LookupClassCategoryDescription(rec.ClassCat, _globals.Agency, _clientStore.ClientQueryDb, _isTitleCaseRequired);
                case "RAL_CONNEC":
                    return rec.Connect.EqualsIgnoreCase("X") ? "Y" : "N";
                case "RAL_DAIRPT":
                    return AportLookup.LookupAport(_masterStore, rec.Destinat, rec.Mode, _globals.Agency);
                case "RAL_DEPDOW":
                    return rec.RDepDate.GetValueOrDefault().DayOfWeek.ToString().Left(3);
                case "RAL_DMTCTY":
                    return LookupFunctions.LookupAirMetro(_masterStore, rec.Destinat, "C", rec.Mode);
                case "RAL_DMTSTT":
                    return LookupFunctions.LookupAirMetro(_masterStore, rec.Destinat, "S", rec.Mode);
                case "RAL_DSTRGN":
                    return LookupFunctions.LookupRegionCode(rec.Destinat, rec.Mode, _masterStore);
                case "RAL_DTCTRY":
                    return LookupFunctions.LookupCountry(_masterStore, rec.Destinat, _isTitleCaseRequired, rec.Mode);
                case "RAL_OAIRPT":
                    return AportLookup.LookupAport(_masterStore, rec.Origin, rec.Mode, _globals.Agency);
                case "RAL_OMTSTT":
                    return LookupFunctions.LookupAirMetro(_masterStore, rec.Origin, "S", rec.Mode);
                case "RAL_OPRNAM":
                    return LookupFunctions.LookupAline(_masterStore, rec.Airline, rec.Mode, isTitleCase: _isTitleCaseRequired);
                case "RAL_ORCTRY":
                    return LookupFunctions.LookupCountry(_masterStore, rec.Origin, _isTitleCaseRequired, rec.Mode);
                case "RAL_ORGRGN":
                    return LookupFunctions.LookupRegionCode(rec.Origin, rec.Mode, _masterStore);
                case "RAL_ORMCTY":
                    return LookupFunctions.LookupAirMetro(_masterStore, rec.Origin, "C", rec.Mode);
                case "RAL_SGCNTR":
                    return "000";
                case "RAL_TRIATA":
                    return LookupFunctions.LookupAlineNbr(_masterStore, rec.Airline);
                default:
                    return ReportBuilder.GetValueAsString(rec, column.Name);
            }
        }

        public string HandleLookupFieldLegRailLegacy(UserReportColumnInformation column, RawData mainRec, int seqNo)
        {
            //It appears that for legs, sequence numbers start at 1, not zero. 
            seqNo++;
            var rec = _userDefinedParams.RailLegLookup[mainRec.RecKey].FirstOrDefault(s => (_segmentOrLeg == SegmentOrLeg.Segment && s.Seg_Cntr == seqNo) || (_segmentOrLeg == SegmentOrLeg.Leg && s.SeqNo == seqNo));

            if (rec == null) return string.Empty;

            switch (column.Name)
            {
                case "RAL_ACTFAR":
                case "RAL_SGCOST":
                    return (rec.ActFare + rec.MiscAmt).ToString();
                case "RAL_CPM":
                    return (rec.Miles == 0) ? "0.00" : (mainRec.PlusMin * (rec.ActFare + rec.MiscAmt) / rec.Miles).ToString();
                case "RAL_MILES":
                    return (Math.Abs(rec.Miles) * mainRec.PlusMin).ToString();
                case "RAL_ACARC2":
                    return rec.AltCarCo2.ToString();
                case "RAL_AIRCO2":
                    return rec.AirCo2.ToString();
                case "RAL_ARALC2":
                    return rec.AltRailCo2.ToString();
                case "RAL_ARRDAT":
                    return rec.RArrDate.GetValueOrDefault().ToShortDateString();
                case "RAL_ARRTIM":
                    return _isDdTime ? SharedProcedures.ConvertTime(rec.ArrTime) : rec.ArrTime;
                case "RAL_CLASS":
                    return rec.ClassCode;
                case "RAL_CLASCT":
                    return rec.ClassCat;
                case "RAL_DEPDAT":
                    return rec.RDepDate.GetValueOrDefault().ToShortDateString();
                case "RAL_DEPTIM":
                    return _isDdTime ? SharedProcedures.ConvertTime(rec.DepTime) : rec.DepTime;
                case "RAL_DITCOD":
                    return rec.DitCode;
                case "RAL_DSTNAT":
                    //* 06/25/09 - SPECIAL HANDLING: USE origdest WHEN PRESENT. 
                    return !string.IsNullOrEmpty(rec.Origdest) ? rec.Origdest : rec.Destinat;
                case "RAL_FARBAS":
                    return rec.Farebase;
                case "RAL_FLTNO":
                    return rec.fltno;
                case "RAL_MODE":
                    return rec.Mode;
                case "RAL_MSCAMT":
                    return rec.MiscAmt.ToString();
                case "RAL_OPRCOD":
                    //* 06/25/09 - SPECIAL HANDLING: USE origcarr WHEN PRESENT
                    return !string.IsNullOrEmpty(rec.Origcarr) ? rec.Origcarr : rec.Airline;
                case "RAL_ORIGIN":
                    //* 06/25/09 - SPECIAL HANDLING: USE origorigin WHEN PRESENT. 
                    return !string.IsNullOrEmpty(rec.Origorigin) ? rec.Origorigin : rec.Origin;
                case "RAL_PLSMIN":
                    return rec.Rplusmin.ToString();
                case "RAL_SEAT":
                    return rec.Seat;
                case "RAL_SEQNO":
                    return rec.SeqNo.ToString();
                case "RAL_SGSTAT":
                    return rec.Segstatus;
                case "RAL_TKTDES":
                    return rec.Tktdesig;
                case "RAL_ARDTTM":
                    return rec.RArrDate.GetValueOrDefault().MakeDateTime(rec.ArrTime).ToString(CultureInfo.InvariantCulture);
                case "RAL_RPDTTM":
                    return rec.RDepDate.GetValueOrDefault().MakeDateTime(rec.DepTime).ToString(CultureInfo.InvariantCulture);
                default:
                    return ReportBuilder.GetValueAsString(rec, column.Name);
            }
        }

    }
}
