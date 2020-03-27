using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.TravelerbyCountryReport;
using Domain.Orm.iBankClientQueries;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TravelerByCountry
{
    public class TravByCountry : ReportRunner<RawData, FinalData>
    {
        public UserBreaks UserBreaks { get; set; }

        public bool IsReservationReport { get; set; }
        public bool IncludeOneWay { get; set; }
        private int Udid { get; set; }

        private List<RawData> Destinations { get; set; }

        private readonly TravByCountryCalculations _calc = new TravByCountryCalculations();

        private readonly TravByCountryDataProcessor _processor = new TravByCountryDataProcessor();

        private readonly TravByCountrySqlCreator _creator = new TravByCountrySqlCreator();

        public TravByCountry()
        {
            Destinations = new List<RawData>();
        }

        #region ReportRunner functions
        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        private void SetProperties()
        {
            IsReservationReport = GlobalCalc.IsReservationReport();

            IncludeOneWay = GlobalCalc.IncludeOneWay();

            Udid = GlobalCalc.GetUdidNumber();

            CrystalReportName = _calc.GetCrystalReportName();

        }

        public override bool GetRawData()
        {
            SetProperties();

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false,
                buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false,
                legDit: false, ignoreTravel: true)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }
            BuildWhere.AddSecurityChecks();

            var sql = _creator.Create(BuildWhere.WhereClauseFull, Udid, IsReservationReport);
            RawDataList = RetrieveRawData<RawData>(sql, IsReservationReport, true).ToList();

            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            RawDataList = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);

            PerformCurrencyConversion(RawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            /**NOTE - THE ONLY ROUTING CRITERIA WE CAN ALLOW IS ORIGIN/ DESTINATION * *
            **AIRPORTS, COUNTRIES, REGIONS.WE HAVE TO PROCESS THE WHOLE TRIP, IN**
            ** ORDER TO GET THE DEPARTURE DATE OF THE NEXT SEGMENT, SO WE CAN**
            ** DETERMINE THE LENGTH OF STAY. */

            /**NOW WE NEED TO WALK THROUGH THE CURSOR AND BUILD "DESTINATION" RECORDS**
            ** FOR ALL DESTINATIONS EXCEPT THE RETURN DESTINATION, WHICH IN MOST CASES**
            ** WILL BE THE FINAL SEGMENT.  */

            /** TO BUILD OUR NEW RECORDS, WE ESSENTIALLY NEED TO LOOK AT 2 RECORDS **
            ** TOGETHER, SO WE CAN DETERMINE THE LENGTH OF STAY*/
            var previousRecord = new RawData();

            if (RawDataList.Count == 1)
            {
                if (IncludeOneWay)
                {
                    previousRecord.Days = 0;
                    previousRecord.OneWayTrip = true;

                    Destinations.Add(previousRecord);
                }
            }
            else
            {
                Destinations.AddRange(_processor.ProcessDestinations(RawDataList, IncludeOneWay));
            }

            //Apply routing criteria
            Destinations = BuildWhere.ApplyWhereRoute(Destinations, false, false);

            //Group data into FinalData
            FinalDataList = _processor.GroupFinalData(Destinations, MasterStore, Globals);

            if (!DataExists(FinalDataList)) return false;

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = _calc.GetExportFields().ToList();
                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                    }
                    break;
                default:

                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);
                    ReportSource.SetParameterValue("LLOGGEN1", IncludeOneWay);

                    ReportSource.SetParameterValue("NTOTDAYS", FinalDataList.Sum(s => s.Totdays));
                    ReportSource.SetParameterValue("NTOTCNT", FinalDataList.Sum(s => s.Dispticks));

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            return true;
        }

        #endregion
    }
}
