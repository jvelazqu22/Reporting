using System.Linq;
using System.Collections.Generic;
using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;

using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomTravelersHotelReport;
using Domain.Orm.iBankClientQueries;

using iBank.Services.Implementation.Shared;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.HotelTopBottomTravelers
{
    public class TopTravHotel : ReportRunner<RawData, FinalData>
    {
        public int TotCount;
        public int TotNights;
        public decimal TotCost;
        public decimal TotRate;
        public int TotBookCount;

        private List<string> _exportFields = new List<string> { "PassLast", "PassFrst", "Stays", "Nights", "HotelCost", "BookRate", "BookCnt", "AvgBook" };

    public TopTravHotel()
        {
            CrystalReportName = "ibTopTravHotel";
        }

        public override bool InitialChecks()
        {

            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return true;

        }

        public override bool GetRawData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            //if we apply routing in both directions "SpecRouting" is true. 
            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: true,
                buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false);

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            // Fetch raw data
            var udidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1);
            var sql = SqlBuilder.GetSql(udidNumber > 0, GlobalCalc.IsReservationReport(), BuildWhere.WhereClauseFull);
            RawDataList = RetrieveRawData<RawData>(sql, GlobalCalc.IsReservationReport(), false).ToList();

            // Applies Check-in date filter if there are advanced paramaters
            var dateTypeParm = Globals.GetParmValue(WhereCriteria.DATERANGE).TryIntParse(7);
            if (Globals.AdvancedParameters.Parameters.Count > 0 && dateTypeParm.Equals(DateType.HotelCheckInDate))
            {
                RawDataList = RawDataList.Where(s => s.dateIn >= Globals.BeginDate.ToDateTimeSafe() && s.dateIn <= Globals.EndDate.ToDateTimeSafe()).ToList();
            }
            
            // Data checks
            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            PerformCurrencyConversion(RawDataList);
            return true;
        }

        public override bool ProcessData()
        {
            // Group raw data and transform into final data
            FinalDataList = RawDataList.GroupBy(s => new { s.PassLast, s.PassFrst, s.BookRate }, (key, recs) =>
              {
                  var reclist = recs.ToList();
                  return new GroupedData
                  {
                      PassLast = key.PassLast,
                      PassFrst = key.PassFrst,
                      BookRate = key.BookRate,
                      Stays = reclist.Sum(s => s.HPlusMin),
                      Nights = reclist.Sum(s => s.Nights * s.Rooms * s.HPlusMin),
                      HotelCost = reclist.Sum(s => s.Nights * s.Rooms * s.BookRate),
                      SumBookRate = reclist.Sum(s => s.BookRate),
                  };
              })
              .GroupBy(s => new { s.PassLast, s.PassFrst }, (key, recs) =>
              {
                  var reclist = recs.ToList();
                  return new FinalData
                  {
                      PassLast = key.PassLast,
                      PassFrst = key.PassFrst,
                      Stays = reclist.Sum(s => s.Stays),
                      Nights = reclist.Sum(s => s.Nights),
                      HotelCost = reclist.Sum(s => s.HotelCost),
                      BookRate = reclist.Sum(s => s.SumBookRate),
                      BookCnt = reclist.Sum(s => s.BookRate == 0 ? 0 : s.Stays),
                  };
              }).ToList();

            // Calculate total values
            TotCount = FinalDataList.Sum(s => s.Stays);
            TotNights = FinalDataList.Sum(s => s.Nights);
            TotCost = FinalDataList.Sum(s => s.HotelCost);
            TotRate = FinalDataList.Sum(s => s.BookRate);
            TotBookCount = FinalDataList.Sum(s => s.BookCnt);

            // Sort data list to find top results
            FinalDataList = TopTravHotelHelpers.SortData(FinalDataList, Globals);

            // Make sure there is still data
            return DataExists(FinalDataList) ? true : false;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                // Output to xlsx/csv
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, _exportFields, Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, _exportFields, Globals);
                    }
                    break;
                default:
                    // Output to PDF
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);

                    ReportSource.SetParameterValue("nTotCnt", TotCount);
                    ReportSource.SetParameterValue("nTotNites", TotNights);
                    ReportSource.SetParameterValue("nTotCost", TotCost);
                    ReportSource.SetParameterValue("nTotRate", TotRate);
                    ReportSource.SetParameterValue("nTotBookCnt", TotBookCount);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            return true;
        }
    }
}
