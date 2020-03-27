using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;

using iBank.Services.Implementation.Shared;

using System.Linq;

using Domain.Models.ReportPrograms.SameCityReport;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.SameCity
{
    public class SameCity : ReportRunner<RawData, FinalData>
    {
        public string UdidNumber { get; set; }
        private bool IsReservationReport { get; set; }

        private readonly SameCitySqlCreator _creator = new SameCitySqlCreator();

        private readonly SameCityDataProcessor _processor = new SameCityDataProcessor();

        private readonly SameCityCalculations _calc = new SameCityCalculations();

        public SameCity()
        {
        }

        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!Globals.BeginDate.HasValue && Globals.EndDate.HasValue)
            {
                Globals.BeginDate = Globals.EndDate;
            }

            if (Globals.BeginDate.HasValue && !Globals.EndDate.HasValue)
            {
                Globals.EndDate = Globals.BeginDate;
            }

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        private void SetProperties()
        {
            CrystalReportName = _calc.GetCrystalReportName();
            IsReservationReport = GlobalCalc.IsReservationReport();
        }

        public override bool GetRawData()
        {
            SetProperties();
            var whereTrip = string.Empty;
            
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (Globals.GetParmValue(WhereCriteria.DATERANGE) == "5")
            {
                if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: false, inMemory: true, isRoutingBidirectional: false,legDit: true, ignoreTravel: false)) return false;

                //whereTrip = " and rarrdate between '" + Globals.BeginDate.Value.AddDays(-5).ToShortDateString() + "' and '" +
                //               Globals.EndDate.Value.AddDays(5).ToShortDateString() + " 11:59:59 PM' ";

                whereTrip = _creator.CreateWhereTripClause(Globals.BeginDate.Value, Globals.EndDate.Value);
            }
            else
            {
                if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, true, true, false, false, true, true, true, false, true, false)) return false;
            }

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();
            
            var udid = GlobalCalc.GetUdidNumber();
            var sql = _creator.Create(BuildWhere.WhereClauseFull, udid, IsReservationReport, whereTrip);
            RawDataList = RetrieveRawData<RawData>(sql, IsReservationReport, true).ToList();
            
            if (!DataExists(RawDataList)) return false;

            var collapsedData = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Last);

            //Apply Where Route to the collapsed data
            RawDataList = GlobalCalc.IsAppliedToLegLevelData() ? BuildWhere.ApplyWhereRoute(collapsedData, true) : BuildWhere.ApplyWhereRoute(collapsedData, false);

            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            return true;
        }

        public override bool ProcessData()
        {
            //**WE CANNOT APPLY "# OF TRAVELERS TO A CITY **
            //**IN A DAY" CRITERIA UNTIL WE SUMMARIZE.    **
            var numberOfTravelers = _calc.GetNumberOfTravelers(Globals);
            
            FinalDataList = _processor.MapRawToFinalData(RawDataList, MasterStore, Globals).ToList();

            if (!IsReservationReport)
            {
                //*IF THE REPORT DATA CONTAINS CREDIT RECORDS, LOOK FOR THE MATCHING
                //* INVOICE RECORD.REMOVE BOTH THE INVOICE AND THE CREDIT.
                //* NOTE: PREVIEW DATA DOES NOT HAVE CREDITS
                FinalDataList = _processor.RemoveMatchingCreditAndInvoiceRecords(FinalDataList).ToList();
            }

            //ONLY REPORT CITIES HAVING > 1 TRAVELER TO A CITY IN A DAY.
            if (numberOfTravelers > 1)
            {
                FinalDataList = _processor.FilterToCitiesWithMoreThanOneTraveler(FinalDataList, numberOfTravelers).ToList();
            }
            
            if (!DataExists(FinalDataList)) return false;

            return IsUnderOfflineThreshold(FinalDataList);
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = _calc.GetExportFields().ToList();

                    if (Globals.OutputFormat == DestinationSwitch.Xls)
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                    }
                    else
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                    }
                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);
                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    //Generate a PDF file. 
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }
    }
}
