using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using System.Collections.Generic;
using System.Linq;
using Domain;
using iBank.Entities.MasterEntities;
using Domain.Helper;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public class SwitchManager
    {
        private List<UserReportColumnInformation> _columns;
        private ReportGlobals _globals;

        public SwitchManager(List<UserReportColumnInformation> columns, ReportGlobals globals)
        {
            _columns = columns;
            _globals = globals;
        }
        public bool CarSwitch { get; set; }
        public bool HotelSwitch { get; set; }
        public bool LegSwitch { get; set; }
        public bool ServiceFeeSwitch { get; set; }
        public bool RailLegSwitch { get; set; }
        public bool AirLegSwitch { get; set; }
        public bool MiscSegmentsSwitch { get; set; }
        public bool TripSwitch { get; set; }
        public bool ChangeLogSwitch { get; set; }
        public bool MarketSegmentsSwitch { get; set; }
        public bool TravelAuthorizersSwitch { get; set; }
        public bool TraveAuthSwitch { get; set; }
        public bool TripTlsSwitch { get; set; }
        public bool HotelSwitch2 { get; set; }
        public bool CarSwitch2 { get; set; }
        public bool LegSwitch2 { get; set; }
        public bool ServiceFeeSwitch2 { get; set; }
        public bool HaveRoomType { get; set; }
        public bool NeedRoomType { get; set; }
        public bool MiscSegmentTourSwitch { get; set; }
        public bool MiscSegmentCruiseSwitch { get; set; }
        public bool MiscSegmentLimoSwitch { get; set; }
        public bool UdidSwitch { get; set; }
        public bool MiscSegmentRalSwitch { get; set; }
        public bool LongestSegSwitch { get; set; }
        //TODO: remove when remove feature flag RoutingUseTripsDerivedDataTable
        public bool TransIdGsaSwitch { get; set; } = false;

        //Remove TripSwitch check because we use this program to process COMBDET, AIR/CAR/HOTEL only type too    
        public bool HotelOnly => HotelSwitch  && !LegSwitch && !CarSwitch && !ServiceFeeSwitch && !RailLegSwitch && !AirLegSwitch;

        public bool CarOnly => CarSwitch && !LegSwitch && !HotelSwitch && !ServiceFeeSwitch && !RailLegSwitch && !AirLegSwitch;

        public void PopulateSwitches(IList<collist2> columnList)
        {
            var colsToCheck = new List<string> { "TRIPS", "IBTRIPS", "HIBTRIPS", "TRIPTLS" };
            TripSwitch = _columns.Any(s => colsToCheck.Contains(s.TableName)) || _columns.Any(s => s.TlsOnly);
            TripTlsSwitch = _columns.Any(s => s.TlsOnly);

            colsToCheck = new List<string> { "LEGS", "IBLEGS", "HIBLEGS", "AIRLEG", "RAIL" };
            LegSwitch = _columns.Any(s => colsToCheck.Contains(s.TableName)) || _globals.ProcessKey == (int)ReportTitles.AirUserDefinedReports;

            colsToCheck = new List<string> { "AUTO", "IBCAR", "HIBCARS" };
            CarSwitch = _columns.Any(s => colsToCheck.Contains(s.TableName)) || _globals.ProcessKey == (int)ReportTitles.CarUserDefinedReports;

            colsToCheck = new List<string> { "HOTEL", "IBHOTEL", "HIBHOTEL" };
            HotelSwitch = _columns.Any(s => colsToCheck.Contains(s.TableName)) || _globals.ProcessKey == (int)ReportTitles.HotelUserDefinedReports;

            colsToCheck = new List<string> { "HTLAVGRATE", "HTLCOUNT" };
            HotelSwitch2 = _columns.Any(s => colsToCheck.Contains(s.Name));//Hotel data at the trip level

            colsToCheck = new List<string> { "CARAVGRATE", "CARCOUNT" };
            CarSwitch2 = _columns.Any(s => colsToCheck.Contains(s.Name));//Car data at the trip level

            LegSwitch2 = _columns.Any(s => s.Name.EqualsIgnoreCase("FLTCOUNT"));
            ServiceFeeSwitch2 = _columns.Any(s => s.Name.EqualsIgnoreCase("INVAMTNFEE"));

            colsToCheck = new List<string> { "SVCFEE", "HIBSVCFEES" };
            ServiceFeeSwitch = _columns.Any(s => colsToCheck.Contains(s.TableName)) || _globals.ProcessKey == (int)ReportTitles.ServiceFeeUserDefinedReports;

            AirLegSwitch = _columns.Any(s => s.TableName.EqualsIgnoreCase("AIRLEG"));
            RailLegSwitch = _columns.Any(s => s.TableName.EqualsIgnoreCase("RAIL"));
            MiscSegmentsSwitch = _columns.Any(s => s.TableName.EqualsIgnoreCase("MISCSEGS"));

            ChangeLogSwitch = _columns.Any(s => s.TableName.EqualsIgnoreCase("CHGLOG")) || _columns.Any(s => s.Name.EqualsIgnoreCase("CANCELCODE"));

            //CPPTKTINDR needs to load MarketSegmentDataList          
            MarketSegmentsSwitch = _columns.Any(s => s.TableName.EqualsIgnoreCase("ONDMSEGS"))
                || _columns.Any(s => s.Name.EqualsIgnoreCase("CPPTKTINDR"));

            TravelAuthorizersSwitch = _columns.Any(s => s.TableName.EqualsIgnoreCase("AUTHRZR"));
            TraveAuthSwitch = _columns.Any(s => s.TableName.EqualsIgnoreCase("TRAVAUTH"));

            MiscSegmentTourSwitch = _columns.Any(s => s.TableName.EqualsIgnoreCase("MSTUR"));
            MiscSegmentCruiseSwitch = _columns.Any(s => s.TableName.EqualsIgnoreCase("MSSEA"));
            MiscSegmentLimoSwitch = _columns.Any(s => s.TableName.EqualsIgnoreCase("MSLIM"));
            MiscSegmentRalSwitch = _columns.Any(s => s.TableName.EqualsIgnoreCase("MSRAL"));

            HaveRoomType = _columns.Any(s => s.TableName.EqualsIgnoreCase("ROOMTYPE"));

            LegSwitch2 = LegSwitch2 || _globals.AdvancedParameters.Parameters.Any(s => s.FieldName.EqualsIgnoreCase("FLTCOUNT"));
            LegSwitch2 = LegSwitch2 || _globals.AdvancedParameters.Parameters.Any(s => s.FieldName.EqualsIgnoreCase("CARCOUNT"));
            NeedRoomType = _globals.AdvancedParameters.Parameters.Any(s => s.FieldName.EqualsIgnoreCase("ROOMTYPE"));
            LegSwitch2 = LegSwitch2 || _globals.AdvancedParameters.Parameters.Any(s => s.FieldName.EqualsIgnoreCase("HTLCOUNT")) || NeedRoomType;

            TripTlsSwitch = TripTlsSwitch || _globals.AdvancedParameters.Parameters.Join(columnList.Where(s => s.tlsonly), a => a.FieldName.Trim(), c => c.colname.Trim(), (a, c) => a.FieldName).Any();

            UdidSwitch = _columns.Any(s => s.Name.Left(4).EqualsIgnoreCase("UDID"));

            colsToCheck = new List<string> { "LGTPAIR", "LGTPAIRDES" };
            LongestSegSwitch = _columns.Any(s => colsToCheck.Contains(s.Name));//market segment data at the trip level

            var isGsaAgency = _globals.Agency.Equals("GSA");
            var isPreview = _globals.ParmValueEquals(WhereCriteria.PREPOST, "1");
            if (isGsaAgency && !isPreview && Features.GsaTripTransactionIdFeatureFlag.IsEnabled()) TransIdGsaSwitch = _columns.Any(w => w.Name.EqualsIgnoreCase("TRANSID"));
        }
    }
}
