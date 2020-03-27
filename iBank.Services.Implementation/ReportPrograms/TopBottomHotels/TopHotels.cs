using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomHotelsReport;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomHotels
{
    public class TopHotels : ReportRunner<RawData, FinalData>
    {
        private DateTime _begDate;
        private DateTime _endDate;
        private DateTime _begDate2;
        private DateTime _endDate2;
        private string _groupBy;
        private string _sortBy;

        //parameters
        private string _subTitle;
        private string _catDesc1;
        private string _catDesc2;

        private decimal _totCnt;
        private decimal _totNites;
        private decimal _totNzNites;
        private decimal _totCost;
        private decimal _totRate;
        private decimal _totBookCnt;

        private bool _secondRange;
        private string _dateDesc1;
        private string _dateDesc2;

        private decimal _totCnt2;
        private decimal _totNites2;
        private decimal _totNzNites2;
        private decimal _totCost2;

        private List<GroupedRawData> _groupedRawData;
        private List<GroupedRawData> _groupedRawData2;

        public TopHotels()
        {
            CrystalReportName = "ibTopHotels";
            _catDesc1 = string.Empty;
            _catDesc2 = string.Empty;
            _subTitle = string.Empty;
            _begDate2 = DateTime.MinValue;
            _endDate2 = DateTime.MinValue;
        }

        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            var begDate2 = Globals.GetParmValue(WhereCriteria.BEGDATE2);
            var endDate2 = Globals.GetParmValue(WhereCriteria.ENDDATE2);
            if (!string.IsNullOrEmpty(begDate2)) _begDate2 = begDate2.ToDateFromiBankFormattedString().Value;

            if (!string.IsNullOrEmpty(endDate2)) _endDate2 = endDate2.ToDateFromiBankFormattedString().Value;

            if (_begDate2 > _endDate2)
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = "Your comparison date range doesn't make sense.";
                return false;
            }

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            var outputType = Globals.GetParmValue(WhereCriteria.OUTPUTTYPE);
            _groupBy = Globals.GetParmValue(WhereCriteria.GROUPBY);
            if ("4,6,RG,XG".Contains(outputType) && "5,6,7,8".Contains(_groupBy))
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = "Output to Graph option for 2-level group-by is not supported.";
                return false;
            }

            return true;
        }

        public override bool GetRawData()
        {
            if (!GetRawDataGrouped(true)) return false;
            _secondRange = _begDate2 != DateTime.MinValue && _endDate2 != DateTime.MinValue && !"5,6,7,8".Contains(_groupBy);
            _begDate = Globals.BeginDate.Value;
            _endDate = Globals.EndDate.Value;
            if (_secondRange)
            {
                _dateDesc1 = Globals.BuildDateDesc();
                Globals.BeginDate = _begDate2;
                Globals.EndDate = _endDate2;
                GetRawDataGrouped(false);
                _dateDesc2 = Globals.BuildDateDesc();
            }
            return true;
        }

        public override bool ProcessData()
        {
            var groupedByHotel = new TopBottomHotelFinalGroupData().GroupBy(_groupBy, ref _groupedRawData, MasterStore, Globals);

            var howMany = Globals.GetParmValue(WhereCriteria.HOWMANY).TryIntParse(0);
            _sortBy = Globals.GetParmValue(WhereCriteria.SORTBY);

            FinalDataList = new TopBottomHotelFinalGroupData().GetFinalDataList(_groupBy, groupedByHotel, _sortBy, Globals, howMany, MasterStore);

            if (!DataExists(FinalDataList)) return false;
            if (_secondRange)
            {
                var groupedByHotel2 = new TopBottomHotelFinalGroupData().GroupBy(_groupBy, ref _groupedRawData2, MasterStore, Globals).OrderBy(x => x.Category);

                //join first date range with second
                foreach (var row in FinalDataList)
                {
                    var rows = groupedByHotel2.Where(s => s.Category.EqualsIgnoreCase(row.Category)).ToList();

                    if (rows.Any())
                    {
                        row.Stays2 = rows.Sum(s => s.Stays);
                        row.Nights2 = rows.Sum(s => s.Nights);
                        row.Hotelcost2 = rows.Sum(s => s.HotelCost);
                        var bookCnt = rows.Sum(s => s.BookCnt);
                        row.Avgbook2 = bookCnt == 0 ? 0 : rows.Sum(s => s.BookRate) / bookCnt;
                        row.Nznights2 = rows.Sum(s => s.NzNights);
                    }
                }
                //need to add category in groupedByHotel2 not in groupedByHotel
                foreach (var row in groupedByHotel2)
                {
                    var rows = groupedByHotel.Where(s => s.Category.EqualsIgnoreCase(row.Category)).ToList();
                    var newRow = new FinalData();
                    if (!rows.Any())
                    {
                        newRow.Category = row.Category;
                        newRow.Stays2 = row.Stays;
                        newRow.Nights2 = row.Nights;
                        newRow.Hotelcost2 = row.HotelCost;                 
                        var bookCnt = row.BookCnt;
                        newRow.Avgbook2 = bookCnt == 0 ? 0 : row.BookRate / bookCnt;
                        newRow.Nznights2 = row.NzNights;
                        FinalDataList.Add(newRow);
                    }
                }
                _totCnt2 = FinalDataList.Sum(s => s.Stays2);
                _totNites2 = FinalDataList.Sum(s => s.Nights2);
                _totNzNites2 = FinalDataList.Sum(s => s.Nznights2);
                _totCost2 = FinalDataList.Sum(s => s.Hotelcost2);
            }

            if ("5,6,7,8".Contains(_groupBy))
            {
                _totCnt = FinalDataList.Sum(s => s.Stays2);     
                _totNites = FinalDataList.Sum(s => s.Nights2);
                _totNzNites = FinalDataList.Sum(s => s.Nznights2);
                _totCost = FinalDataList.Sum(s => s.Hotelcost2);
                _totRate = FinalDataList.Sum(s => s.Bookrate2);
                _totBookCnt = FinalDataList.Sum(s => s.Bookcnt2);
            }
            else
            {
                _totCnt = FinalDataList.Sum(s => s.Stays);
                _totNites = FinalDataList.Sum(s => s.Nights);
                _totNzNites = FinalDataList.Sum(s => s.Nznights);
                _totCost = FinalDataList.Sum(s => s.Hotelcost);
                _totRate = FinalDataList.Sum(s => s.Bookrate);
                _totBookCnt = FinalDataList.Sum(s => s.Bookcnt);

                FinalDataList = TopHotelHelpers.SortFinalData(_sortBy, FinalDataList, Globals.ParmValueEquals(WhereCriteria.RBSORTDESCASC, "1"));
                if (howMany > 0) FinalDataList = FinalDataList.Take(howMany).ToList();
            }

            // When the filter is done by metro region by chain or "Metro Region / Hotel Hotel Property", 
            // foxpro only displays 25 results per category the code below limits the .net results to 25 too
            if ("6,7".Contains(_groupBy))
            {
                var categories = FinalDataList.Select(s => s.Category).Distinct().ToList();
                var list = new List<FinalData>();
                foreach (var category in categories)
                {
                    list.AddRange(FinalDataList.Where(w => w.Category.Equals(category)).Take(25));
                }

                FinalDataList = list;
            }
           
           return true;
        }

        public override bool GenerateReport()
        {
            CrystalReportName = TopHotelHelpers.GetReportName(_groupBy, _secondRange);
            
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    ExcelGenerator.ExportData(FinalDataList,_secondRange,Globals);
                    break;
                    
                default:
                    var outputType = Globals.GetParmValue(WhereCriteria.OUTPUTTYPE);
                    if ("4,6,TG,XG".Contains(outputType))
                    {
                        var crystalReportName = CrystalReportName;
                        var generateGraph = new TopBottomHotelGraph().GenerateGraph(_sortBy, FinalDataList, ref crystalReportName, _secondRange, Globals,
                            ReportSource, _catDesc1, _begDate, _begDate2, _endDate, _endDate2);
                        CrystalReportName = crystalReportName;
                        return generateGraph;
                    }

                    GeneratePdfRerport();
                    break;
            }
            return true;
        }

        private void GeneratePdfRerport()
        {
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

            ReportSource.SetParameterValue("cCatDesc", _catDesc1);
            ReportSource.SetParameterValue("cCatDesc2", _catDesc2);
            ReportSource.SetParameterValue("cSubTitle", _subTitle);

            ReportSource.SetParameterValue("nTotNites", _totNites);
            ReportSource.SetParameterValue("nTotNzNites", _totNzNites);
            ReportSource.SetParameterValue("nTotBookCnt", _totBookCnt);
            ReportSource.SetParameterValue("nTotCost", _totCost);
            ReportSource.SetParameterValue("nTotrate", _totRate);
            ReportSource.SetParameterValue("nTotCnt", _totCnt);

            if (CrystalReportName == "ibTopHotels")
            {
                ReportSource.SetParameterValue("lbl100Pct", "100 %");
                ReportSource.SetParameterValue("lbfReportTotals", "Report Totals:");
            }
            if (CrystalReportName == "ibTopHotel2")
            {
                ReportSource.SetParameterValue("nTotNites2", _totNites2);
                ReportSource.SetParameterValue("nTotNzNites2", _totNzNites2);
                ReportSource.SetParameterValue("nTotCost2", _totCost2);
                ReportSource.SetParameterValue("nTotCnt2", _totCnt2);
                ReportSource.SetParameterValue("cDateDesc", _dateDesc1);
                ReportSource.SetParameterValue("cDateDesc2", _dateDesc2);
            }

            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }

        private bool GetRawDataGrouped(bool firstPass)
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, 
                buildHotelWhere: true, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false,
                legDit: false, ignoreTravel: false))
            {
                return false;
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

            var isReservationReport = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");

            var udidNo = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1);

            var sql = SqlBuilder.GetSql(udidNo, isReservationReport, "", BuildWhere);

            var returnAllLegs = !BuildWhere.ReportGlobals.AdvancedParameters.Parameters.Any();
            RawDataList = RetrieveRawData<RawData>(sql, isReservationReport, false, returnAllLegs).ToList();

            if (!DataExists(RawDataList) && firstPass) return false;
            PerformCurrencyConversion(RawDataList);

            new TopBottomHotelRawGroupData().UpdateGroupedRawData(_groupBy, RawDataList, ref _subTitle, ref _catDesc1, ref _catDesc2, isReservationReport, firstPass, ref _groupedRawData, ref _groupedRawData2);

            return true;
        }

    }
}
