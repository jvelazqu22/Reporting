using Domain.Exceptions;

using iBank.Services.Implementation.Classes.Interfaces;
using iBank.Services.Implementation.ReportPrograms;
using iBank.Services.Implementation.ReportPrograms.AccountSummary;
using iBank.Services.Implementation.ReportPrograms.AccountSummaryAir12MonthTrend;
using iBank.Services.Implementation.ReportPrograms.AdvanceBookAir;
using iBank.Services.Implementation.ReportPrograms.AgentAirActivity;
using iBank.Services.Implementation.ReportPrograms.AgentProductivity;
using iBank.Services.Implementation.ReportPrograms.AgentSummary;
using iBank.Services.Implementation.ReportPrograms.AirActivity;
using iBank.Services.Implementation.ReportPrograms.AirActivityByUdid;
using iBank.Services.Implementation.ReportPrograms.AirFareSavings;
using iBank.Services.Implementation.ReportPrograms.AirTopBottomSegment;
using iBank.Services.Implementation.ReportPrograms.Arrival;
using iBank.Services.Implementation.ReportPrograms.BroadcastDetails;
using iBank.Services.Implementation.ReportPrograms.BroadcastStatus;
using iBank.Services.Implementation.ReportPrograms.CarActivity;
using iBank.Services.Implementation.ReportPrograms.CarFareSavings;
using iBank.Services.Implementation.ReportPrograms.CarrierConcentration;
using iBank.Services.Implementation.ReportPrograms.CarTopBottomAccounts;
using iBank.Services.Implementation.ReportPrograms.CCRecon;
using iBank.Services.Implementation.ReportPrograms.ClassOfService;
using iBank.Services.Implementation.ReportPrograms.CO2AirSummary;
using iBank.Services.Implementation.ReportPrograms.CO2CombinedSummary;
using iBank.Services.Implementation.ReportPrograms.ConcurrentSegmentsBooked;
using iBank.Services.Implementation.ReportPrograms.CreditCardDetail;
using iBank.Services.Implementation.ReportPrograms.Departures;
using iBank.Services.Implementation.ReportPrograms.DocumentDeliveryLog;
using iBank.Services.Implementation.ReportPrograms.EventLog;
using iBank.Services.Implementation.ReportPrograms.ExceptAir;
using iBank.Services.Implementation.ReportPrograms.ExceptCar;
using iBank.Services.Implementation.ReportPrograms.ExceptHotel;
using iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryByHomeCountry;
using iBank.Services.Implementation.ReportPrograms.ExecutiveSummarywithGraphs;
using iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear;
using iBank.Services.Implementation.ReportPrograms.HotelAccountTopBottom;
using iBank.Services.Implementation.ReportPrograms.HotelActivity;
using iBank.Services.Implementation.ReportPrograms.HotelFareSavings;
using iBank.Services.Implementation.ReportPrograms.HotelTopBottomTravelers;
using iBank.Services.Implementation.ReportPrograms.Invoice;
using iBank.Services.Implementation.ReportPrograms.Market;
using iBank.Services.Implementation.ReportPrograms.MissedHotelOpportunities;
using iBank.Services.Implementation.ReportPrograms.NegotiatedSavings;
using iBank.Services.Implementation.ReportPrograms.PassengersOnPlane;
using iBank.Services.Implementation.ReportPrograms.PTARequestActivity;
using iBank.Services.Implementation.ReportPrograms.PTATravelersDetail;
using iBank.Services.Implementation.ReportPrograms.PublishedSavings;
using iBank.Services.Implementation.ReportPrograms.QuickSummary;
using iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth;
using iBank.Services.Implementation.ReportPrograms.RailActivity;
using iBank.Services.Implementation.ReportPrograms.RailTopBottomCityPair;
using iBank.Services.Implementation.ReportPrograms.ReportLog;
using iBank.Services.Implementation.ReportPrograms.SameCity;
using iBank.Services.Implementation.ReportPrograms.SavedReportList;
using iBank.Services.Implementation.ReportPrograms.ServiceFeeDetailByTransaction;
using iBank.Services.Implementation.ReportPrograms.ServiceFeeDetailByTrip;
using iBank.Services.Implementation.ReportPrograms.SvcFeeSumTran;
using iBank.Services.Implementation.ReportPrograms.TopAccountAir;
using iBank.Services.Implementation.ReportPrograms.TopBottomCars;
using iBank.Services.Implementation.ReportPrograms.TopBottomCityPair;
using iBank.Services.Implementation.ReportPrograms.TopBottomCostCenter;
using iBank.Services.Implementation.ReportPrograms.TopBottomDestinations;
using iBank.Services.Implementation.ReportPrograms.TopBottomExceptionReason;
using iBank.Services.Implementation.ReportPrograms.TopBottomHotels;
using iBank.Services.Implementation.ReportPrograms.TopBottomTravelerAir;
using iBank.Services.Implementation.ReportPrograms.TopBottomTravelerCar;
using iBank.Services.Implementation.ReportPrograms.TopBottomUdids;
using iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers;
using iBank.Services.Implementation.ReportPrograms.TopTravelersAudited;
using iBank.Services.Implementation.ReportPrograms.TransactionSummary;
using iBank.Services.Implementation.ReportPrograms.TravAuthKpi;
using iBank.Services.Implementation.ReportPrograms.TravelAuditReasonsbyMonth;
using iBank.Services.Implementation.ReportPrograms.TravelDetail;
using iBank.Services.Implementation.ReportPrograms.TravelDetail.TravelDetailByUdid;
using iBank.Services.Implementation.ReportPrograms.TravelerByCountry;
using iBank.Services.Implementation.ReportPrograms.TravelManagementSummary;
using iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesAir;
using iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesMeetAndGreet;
using iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesSendOff;
using iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesTypeSummary;
using iBank.Services.Implementation.ReportPrograms.TripDuration;
using iBank.Services.Implementation.ReportPrograms.UpcomingPlans;
using iBank.Services.Implementation.ReportPrograms.UserDefined;
using iBank.Services.Implementation.ReportPrograms.UserDefinedLayout;
using iBank.Services.Implementation.ReportPrograms.UserList;
using iBank.Services.Implementation.ReportPrograms.ValidatingCarrier;
using iBank.Services.Implementation.ReportPrograms.WeeklyTravelerActivity;
using iBank.Services.Implementation.ReportPrograms.WtsFareSavings;
using iBank.Services.Implementation.ReportPrograms.XmlExtract;
using iBank.Services.Implementation.ReportPrograms.TravelOptixReport;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

using iBankDomain.Interfaces;
using Domain.Helper;

namespace iBank.Services.Implementation.Utilities
{
    public class ReportRunnerFactory : IFactory<IReportRunner>
    {
        private int ProcessKey { get; set; }
        private BuildWhere BuildWhere { get; set; }
        private LoadedListsParams _loadedParams;

        public ReportRunnerFactory(int processKey, BuildWhere buildWhere, LoadedListsParams loadedParams)
        {
            ProcessKey = processKey;
            BuildWhere = buildWhere;
            _loadedParams = loadedParams;
        }

        public IReportRunner Build()
        {
            //Each report has a Process Id that determines what report program it should run. 
            switch (ProcessKey)
            {
                case 1:
                    return new SavedRptList(); //TODO: Doesn't actually work yet
                case 2:
                    return new CustAct2 { BuildWhere = this.BuildWhere };
                case 3:
                    return new RailActivity { BuildWhere = this.BuildWhere };
                case 4:
                    return new Arrival { BuildWhere = this.BuildWhere };
                case 6:
                    return new Depart { BuildWhere = this.BuildWhere };
                case 8:
                    return new UpcomingPlans { BuildWhere = this.BuildWhere };
                case 9:
                    return new WeeklyTravelerActivity { BuildWhere = this.BuildWhere };
                case 10:
                    return new Market { BuildWhere = this.BuildWhere };
                case 12:
                    return new PaxOnPlane { BuildWhere = this.BuildWhere };
                case 14:
                    return new AdvanceBook { BuildWhere = this.BuildWhere };
                case 16:
                    return new AirActivityUdid { BuildWhere = this.BuildWhere };
                case 18:
                    return new AirFareSavings { BuildWhere = this.BuildWhere };
                case 20:
                    return new SameCity { BuildWhere = this.BuildWhere };
                case 21:
                    return new TripDuration { BuildWhere = this.BuildWhere };
                case 24:
                     return new TravDet2 { BuildWhere = this.BuildWhere };   
                case 23: 
                    return new TravDet1 { BuildWhere = this.BuildWhere };               
                case 27:
                    return new Invoice { BuildWhere = this.BuildWhere };
                case 28:
                    return new ClassofSvc { BuildWhere = this.BuildWhere };
                case 29:
                    return new CarrierConcentration { BuildWhere = this.BuildWhere };
                case 30:
                    return new QView1 { BuildWhere = this.BuildWhere };
                case 31:
                    return new ExecutiveSummaryHomeCountry { BuildWhere = this.BuildWhere };
                case 32:
                    return new Qview2 { BuildWhere = this.BuildWhere };
                case 34:
                    return new ExecutiveSummaryYearToYear { BuildWhere = this.BuildWhere };
                case 36:
                    return new Qview4 { BuildWhere = this.BuildWhere };
                case 38:
                    return new TravelManagementSummary { BuildWhere = this.BuildWhere };
                case 40:
                    return new TopBottomValidatingCarriers { BuildWhere = this.BuildWhere };
                case 42:
                    return new AirTopBottomSegment { BuildWhere = this.BuildWhere };
                case 44:
                    return new TopCityPair { BuildWhere = this.BuildWhere };
                case 46:
                    return new TopCars { BuildWhere = this.BuildWhere };
                case 48:
                    return new TopHotels { BuildWhere = this.BuildWhere };
                case 50:
                    return new TopAccountAir { BuildWhere = this.BuildWhere };
                case 52:
                    return new CarTopBottomAccounts { BuildWhere = this.BuildWhere };
                case 53:
                    return new TopBottomCostCenter { BuildWhere = this.BuildWhere };
                case 54:
                    return new HotelAccountTopBottom { BuildWhere = this.BuildWhere };
                case 55:
                    return new TopTravCar { BuildWhere = this.BuildWhere };
                case 56:
                    return new TopTravAir { BuildWhere = this.BuildWhere };
                case 57:
                    return new TopTravHotel { BuildWhere = this.BuildWhere };
                case 58:
                    return new TopBottomUdids { BuildWhere = this.BuildWhere };
                case 59:
                    return new TopTravAll { BuildWhere = this.BuildWhere };
                case 60:
                case 62:
                    return new TopExceptions { BuildWhere = this.BuildWhere };
                case 63:
                    return new TopTravAllByUdid { BuildWhere = this.BuildWhere };
                case 64:
                    return new TopBottomDestinations { BuildWhere = this.BuildWhere };
                case 66:
                    return new TopCityPairRail { BuildWhere = this.BuildWhere };
                case 70:
                case 72:
                case 74:
                case 76:
                    return new CarActivity{ BuildWhere = this.BuildWhere };
                case 80:
                case 82:
                case 84:
                case 86:
                    return new HotelActivity { BuildWhere = this.BuildWhere };
                case 88:
                    return new MissedHotel { BuildWhere = this.BuildWhere };
                case 90:
                    return new CarSavings { BuildWhere = this.BuildWhere };
                case 92:
                    return new HotelSavings { BuildWhere = this.BuildWhere };
                case 100:
                    return new ExceptAir { BuildWhere = this.BuildWhere };
                case 102:
                    return new ExceptCar { BuildWhere = this.BuildWhere };
                case 104:
                    return new ExceptHotel { BuildWhere = this.BuildWhere };
                case 118:
                    return new EventLog { BuildWhere = this.BuildWhere };
                case 120:
                    return new AgentProductivity { BuildWhere = this.BuildWhere };
                case 122:
                    return new ReportLog { BuildWhere = this.BuildWhere };
                case 124:
                    return new UserList { BuildWhere = this.BuildWhere };
                case 126:
                    return new AgcyValCarr { BuildWhere = this.BuildWhere };
                case 127:
                    return new BroadcastStatus { BuildWhere = this.BuildWhere };
                case 128:
                    return new AgentSummary { BuildWhere = this.BuildWhere };
                case 130:
                    return new AgentAirActivity { BuildWhere = this.BuildWhere };
                case 132:
                    return new AccountSummary { BuildWhere = this.BuildWhere };
                case 134:
                    return new AcctSum2 { BuildWhere = this.BuildWhere };
                case 136:
                    return new BroadcastDetails { BuildWhere = this.BuildWhere };
                case 138:
                    return new TransactionSummary { BuildWhere = this.BuildWhere };
                case 140:
                    return new SvcFeeDetTrip { BuildWhere = this.BuildWhere };
                case 142:
                    return new SvcFeeDetTran { BuildWhere = this.BuildWhere };
                case 144:
                    return new SvcFeeSumTran { BuildWhere = this.BuildWhere };
                case 146:
                    return new CcRecon { BuildWhere = this.BuildWhere };
                case 160:
                    return new PubSave { BuildWhere = this.BuildWhere };
                case 162:
                    return new NegoSave { BuildWhere = this.BuildWhere };
                case 180:
                    return new MeetGreet { BuildWhere = this.BuildWhere };
                case 182:
                    return new TripChange { BuildWhere = this.BuildWhere };
                case 184:
                    return new TripChngSum { BuildWhere = this.BuildWhere };
                case 186:
                    return new SendOff { BuildWhere = this.BuildWhere };
                case 201:
                    return new Overlap { BuildWhere = this.BuildWhere };
                case 205:
                    return new TravByCountry {BuildWhere = this.BuildWhere};
                case 221:
                    return new Co2ExecSum { BuildWhere = this.BuildWhere };
                case 222:
                    return new Co2AirSum { BuildWhere = this.BuildWhere };
                case 241:
                    return new TravAuthStatusDet { BuildWhere = this.BuildWhere };
                case 243:
                    return new TravAuthDetail { BuildWhere = this.BuildWhere };
                case 245:
                    return new TravAuthStatusLog { BuildWhere = this.BuildWhere };
                case 247:
                    return new TravAuthTopOop { BuildWhere = this.BuildWhere };
                case 249:
                    return new TravAuthOopMonth { BuildWhere = this.BuildWhere };
                case 251:
                    return new TravAuthKpi { BuildWhere = this.BuildWhere };
                case 261: 
                    return new TravelAtAGlance { BuildWhere = this.BuildWhere };
                case 281:
                    return new CreditCardDetail { BuildWhere = this.BuildWhere };
                case 508:
                    return new SavedReportList { BuildWhere = this.BuildWhere };
                case 509:
                    return new UserDefinedLayout { BuildWhere = this.BuildWhere };
                case 501:
                case 502:
                case 503:
                case 504:
                case 520:
                    return new UserDefined(_loadedParams) { BuildWhere = this.BuildWhere };
                case 581:
                    return new XmlExtract { BuildWhere = this.BuildWhere };
                case 701:
                    return new WtsFareSavings { BuildWhere = this.BuildWhere };
                case (int)ReportTitles.TravelOptixReport:
                    return new TravelOptix { BuildWhere = this.BuildWhere };
                default:
                    throw new ReportNotSupportedException($"Process key [{ProcessKey}] not supported.");
            }
        }
    }
}
