using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.Graphs;
using Domain.Models.ReportPrograms.TopBottomCars;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomCars
{
    public class TopCars : ReportRunner<RawData, FinalData>
    {
        private TopCarHelpers _topCarHelpers = new TopCarHelpers();

        private DateTime _begDate;
        private DateTime _endDate;
        private DateTime _begDate2;
        private DateTime _endDate2;
        private string _groupBy;
        private string _sortBy;

        private string _subTitle;
        private string _catDesc1;
        private string _catDesc2;

        public decimal TotCnt;
        public decimal TotDays;
        public decimal TotNzDays;
        public decimal TotCost;
        public decimal TotRate;
        public decimal TotBookCnt;

        private bool _secondRange;
        private string _dateDesc1;
        private string _dateDesc2;

        public decimal TotCnt2;
        public decimal TotDays2;
        public decimal TotNzDays2;
        public decimal TotCost2;

        private List<GroupedRawData> _groupedRawData;
        private List<GroupedRawData> _groupedRawData2;

        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;
            if (!IsDateRangeValid()) return false;

            SetDates();

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
                _groupBy = "2";
            }
            _secondRange = _begDate2 != DateTime.MinValue && _endDate2 != DateTime.MinValue;
            if (_secondRange && _groupBy.Equals("4"))
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = "Not a valid combination -- You cannot have a comparison date range if you are grouping by rental cities with a breakdown by car companies.";
                return false;
            }
            return true;
        }

        private void SetDates()
        {
            var begDate = Globals.GetParmValue(WhereCriteria.BEGDATE);
            var endDate = Globals.GetParmValue(WhereCriteria.ENDDATE);
            if (!string.IsNullOrEmpty(begDate))
            {
                _begDate = begDate.ToDateFromiBankFormattedString().Value;
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                _endDate = endDate.ToDateFromiBankFormattedString().Value;
            }

            var begDate2 = Globals.GetParmValue(WhereCriteria.BEGDATE2);
            var endDate2 = Globals.GetParmValue(WhereCriteria.ENDDATE2);
            if (!string.IsNullOrEmpty(begDate2))
            {
                _begDate2 = begDate2.ToDateFromiBankFormattedString().Value;
            }

            if (!string.IsNullOrEmpty(endDate2))
            {
                _endDate2 = endDate2.ToDateFromiBankFormattedString().Value;
            }
        }

        public override bool GetRawData()
        {
            if (!GetRawDataGrouped(true)) return false;

            if (_secondRange)
            {
                _dateDesc1 = Globals.BuildDateDesc();
                _begDate = Globals.BeginDate.Value;
                _endDate = Globals.EndDate.Value;
                Globals.BeginDate = _begDate2;
                Globals.EndDate = _endDate2;
                GetRawDataGrouped(false);
                _dateDesc2 = Globals.BuildDateDesc();
            }
            return true;
        }

        public override bool ProcessData()
        {
            var howMany = Globals.GetParmValue(WhereCriteria.HOWMANY).TryIntParse(0);
            _sortBy = Globals.GetParmValue(WhereCriteria.SORTBY);

            FinalDataList = new TopBottomCarsFinalData().GetFinalDataList(_groupBy, _groupedRawData, howMany, _sortBy, Globals, MasterStore, 
                ref TotCnt, ref TotDays, ref TotNzDays, ref TotCost, ref TotRate, ref TotBookCnt, ref TotCnt2, ref TotDays2, ref TotNzDays2, 
                ref TotCost2, _secondRange, _groupedRawData2);

            if (!DataExists(FinalDataList)) return false;

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFieldList = _topCarHelpers.GetExportFields(_secondRange, _groupBy);

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFieldList, Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals);
                    }
                    break;

                default:
                    var outputType = Globals.GetParmValue(WhereCriteria.OUTPUTTYPE);
                    if ("4,6,TG,XG".Contains(outputType))
                    {
                        return GenerateGraph();
                    }

                    CreatePdf();
                    break;
            }

            return true;
        }

        private void CreatePdf()
        {
            CrystalReportName = _topCarHelpers.GetReportName(_secondRange, _groupBy);
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);

            var missingParameters = CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

            ReportSource.SetParameterValue("cCatDesc", _catDesc1);
            ReportSource.SetParameterValue("cCatDesc2", _catDesc2);
            ReportSource.SetParameterValue("cSubTitle", _subTitle);
            ReportSource.SetParameterValue("nTotDays", TotDays);
            ReportSource.SetParameterValue("nTotNzDays", TotNzDays);
            ReportSource.SetParameterValue("nTotCost", TotCost);
            ReportSource.SetParameterValue("nTotCnt", TotCnt);

            if (CrystalReportName == "ibTopCars")
            {
                ReportSource.SetParameterValue("lbl100Pct", "100 %");
                ReportSource.SetParameterValue("lbfReportTotals", "Report Totals:");
            }

            //if (CrystalReportName == "ibTopCar2")
            if (CrystalReportName == "ibTopCar5")
            {
                ReportSource.SetParameterValue("nTotDays2", TotDays2);
                ReportSource.SetParameterValue("nTNzDays2", TotNzDays2);
                ReportSource.SetParameterValue("nTotCost2", TotCost2);
                ReportSource.SetParameterValue("nTotCnt2", TotCnt2);
                ReportSource.SetParameterValue("cDateDesc1", _dateDesc1);
                ReportSource.SetParameterValue("cDateDesc2", _dateDesc2);
            }
            else
            {
                ReportSource.SetParameterValue("nTotBookCnt", TotBookCnt);
                ReportSource.SetParameterValue("nTotrate", TotRate);
            }
            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }

        private bool GetRawDataGrouped(bool firstPass)
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, true, false, true, false, true, true, false, false,false, false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var udidNo = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1);

            var sqlScript = SqlBuilder.GetSql(udidNo > 0, GlobalCalc.IsReservationReport(), BuildWhere);

            RawDataList = RetrieveRawData<RawData>(sqlScript, GlobalCalc.IsReservationReport(), false).ToList();
            RawDataList = PerformCurrencyConversion(RawDataList);

            if (!DataExists(RawDataList) && firstPass) return false;
            PerformCurrencyConversion(RawDataList);
            var comps = RawDataList.Select(s => s.Company).Distinct().ToList();

            List<GroupedRawData> groupedRawData = new TopBottomCarsRawData().GroupRawData(GlobalCalc, RawDataList, _groupBy, ref _subTitle, ref _catDesc1, ref _catDesc2);

            if (firstPass)
                _groupedRawData = groupedRawData;
            else
                _groupedRawData2 = groupedRawData;

            return true;
        }

        private bool GenerateGraph()
        {
            var graphTitle = "Volume Booked";
            switch (_sortBy)
            {
                case "2":
                    graphTitle = "# of Rentals";
                    break;
                case "3":
                    graphTitle = "# of Days";
                    break;
                case "4":
                    graphTitle = "Avg Booked Rate";
                    break;
            }

            var graphData = FinalDataList.Select(s => new Graph1FinalData
            {
                CatDesc = s.Category.Left(16),
                Data1 = GetGraphData1(s),
                Data2 = GetGraphData2(s)
            }).ToList();

            CrystalReportName = !_secondRange ? "ibGraph1" : "ibGraph2";

            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(graphData);
            
            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            ReportSource.SetParameterValue("cGrDataType", "2,3".Contains(_sortBy) ? "N" : "C");
            ReportSource.SetParameterValue("cCatDesc", _catDesc1);
            ReportSource.SetParameterValue("cSubTitle", graphTitle);
            ReportSource.SetParameterValue("cGrColHdr1", _begDate.ToShortDateString() + " - " + _endDate.ToShortDateString());
            if (_secondRange)
            {
                ReportSource.SetParameterValue("cGrColHdr2", _begDate2.ToShortDateString() + " - " + _endDate2.ToShortDateString());
            }
            CrystalFunctions.CreatePdf(ReportSource, Globals);
            return true;
        }

        private decimal GetGraphData1(FinalData rec)
        {
            switch (_sortBy)
            {
                case "2":
                    return rec.Rentals;
                case "3":
                    return rec.Days;
                case "4":
                    return rec.Avgbook;
                default:
                    return rec.Carcost;
            }
        }

        private decimal GetGraphData2(FinalData rec)
        {
            switch (_sortBy)
            {
                case "2":
                    return rec.Rentals2;
                case "3":
                    return rec.Days2;
                case "4":
                    return rec.Avgbook2;
                default:
                    return rec.Carcost2;
            }
        }

    }
}
