using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.TopBottomValidatingCarriers;
using Domain.Orm.iBankClientQueries;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers
{
    public class TopBottomValidatingCarriers : ReportRunner<RawData, FinalData>
    {
        //Top/Bottom Validating Carriers                              
        //process key: 40

        private int nHowMany;
        private string OrderBy;

        private string cDateDesc1;
        private string cDateDesc2;        
        private string cGrColHdr1;
        private string cGrColHdr2;
        private bool lTravPgBrk;
        public int nTotCnt{ get; set; }
        public decimal nTotChg { get; set; }
        public int nTotCnt2 { get; set; }
        public decimal nTotChg2 { get; set; }
        public DateTime? dBegDate2 { get; set; }
        public DateTime? dEndDate2 { get; set; }

        private bool IsPreview;        
        private bool IsSecondRange;
        private bool IsValCarr;
        private bool IsCtryCode;
        private string cWhereExtraTxt;
        private string WhereText;

        public bool IsGraphOutput { get; set; }
        public DataTypes.GroupBy GroupBy { get; set; }
        public DataTypes.Sort Sort { get; set; }
        public DataTypes.SortBy SortBy { get; set; }

        public List<SubReportData> SubReportDataList;
        
        public List<RawData> RawData2List = new List<RawData>();

        public List<GraphFinalData> GraphFinalData = new List<GraphFinalData>();

        public GraphDataHelper graphDataHelper = new GraphDataHelper();

        public DataHelper dataHelper = new DataHelper();
        private string UdidNumber { get; set; }

        public TopBottomValidatingCarriers()
        {
            CrystalReportName = "ibTopValCarr1";
        }

        public override bool InitialChecks()
        {
            Globals.BeginDate = DateConverter.ReplaceEmptyDate(Globals.BeginDate, Globals.EndDate);

            //if the end date is empty and the begin date is not set the end date to the begin date value
            Globals.EndDate = DateConverter.ReplaceEmptyDate(Globals.EndDate, Globals.BeginDate);

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;
            
            dBegDate2 = Globals.GetParmValue(WhereCriteria.BEGDATE2).ToDateFromiBankFormattedString();
            dEndDate2 = Globals.GetParmValue(WhereCriteria.ENDDATE2).ToDateFromiBankFormattedString(true);
            if (!dataHelper.IsDateRangeValid(dBegDate2, dEndDate2, Globals)) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }
        
        public override bool GetRawData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, true, false, false, false, true, true, false, false, false, false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var whereClause = string.IsNullOrWhiteSpace(BuildWhere.WhereClauseAdvanced)
                ? BuildWhere.WhereClauseFull
                : BuildWhere.WhereClauseFull + " AND " + BuildWhere.WhereClauseAdvanced;

            SetProperties();

            var rawDataHelper = new RawDataHelper(BuildWhere, ClientStore.ClientQueryDb, Globals, IsPreview);
            RawDataList = rawDataHelper.GetRawData(whereClause, IsPreview, UdidNumber);

            cDateDesc1 = Globals.BuildDateDesc();
            cGrColHdr1 = Globals.GetDateRangeDesc(" - ");
            WhereText = Globals.WhereText;
            
            if (!DataExists(RawDataList)) return false;

            PerformCurrencyConversion(RawDataList);

            if (IsSecondRange)
            {
                Globals.BeginDate = dBegDate2;
                Globals.EndDate = dEndDate2;

                if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false,buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false,isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;
                rawDataHelper = new RawDataHelper(BuildWhere, ClientStore.ClientQueryDb, Globals, IsPreview);
                RawData2List = rawDataHelper.GetRawData(whereClause, IsPreview, UdidNumber);
                cDateDesc2 = Globals.BuildDateDesc();
                cGrColHdr2 = Globals.GetDateRangeDesc(" - ");

                PerformCurrencyConversion(RawData2List);
            }

            SubReportDataList = new List<SubReportData>();

            return true;
        }

        public override bool ProcessData()
        {
            var inHomeCtry = Globals.GetParmValue(WhereCriteria.INHOMECTRY).Trim();
            var notCtry = Globals.GetParmValue(WhereCriteria.NOTINHOMECTRY).Trim();
            int mode = Globals.GetParmValue(WhereCriteria.MODE) == "" ? 0 : Convert.ToInt32(Globals.GetParmValue(WhereCriteria.MODE));

            cWhereExtraTxt = dataHelper.GetExtraWhereText(mode, inHomeCtry, notCtry);            

            //RawData filter homectry and mode
            var rawDataHelper = new RawDataHelper(BuildWhere, ClientStore.ClientQueryDb, Globals, IsPreview);
            RawDataList = rawDataHelper.ApplyHomeCountryFilterAndMode(RawDataList, inHomeCtry, notCtry, mode);

            if (IsSecondRange)
            {
                //RawData filter homectry and mode
                RawData2List = rawDataHelper.ApplyHomeCountryFilterAndMode(RawData2List, inHomeCtry, notCtry, mode);
            }
            var finalDataHelper = new FinalDataHelper(Globals, OrderBy, GroupBy, SortBy);
            FinalDataList = finalDataHelper.UnionTwoLists(RawDataList, RawData2List, IsCtryCode, IsValCarr, MasterStore);

            SetTotalValues(FinalDataList);
            FinalDataList = finalDataHelper.GroupFinalData(FinalDataList, IsGraphOutput, Sort, nHowMany);
            FinalDataList = finalDataHelper.UpdateSubTotalRange(FinalDataList);

            var subReportHelper = new SubReportDataHelper();
            SubReportDataList = subReportHelper.SetSubReportData(FinalDataList);

            if (IsGraphOutput) GraphFinalData = graphDataHelper.ConvertToGraphFinalData(FinalDataList, IsCtryCode, OrderBy);

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = dataHelper.GetExportFields(GroupBy, IsCtryCode, IsValCarr, IsSecondRange);

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields.ToList(), Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFields.ToList(), Globals);
                    }
                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;
                    ReportSource = IsGraphOutput ? SetGraphReportSource(rptFilePath) : SetPdfReportSource(rptFilePath);
                    
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }

        private void SetProperties()
        {
            bool lcYTDSort;
            bool lYTDOption;

            UdidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR);
            GroupBy = dataHelper.GetGroupBy(Globals.GetParmValue(WhereCriteria.GROUPBY));
            IsPreview = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");
            
            lYTDOption = Globals.IsParmValueOn(WhereCriteria.CBINCLUDEYTDTOTALS) && dBegDate2 == null && dEndDate2 == null;

            var test = this;
            dataHelper.AdjustDate2Values(Globals.IsParmValueOn(WhereCriteria.CBINCLUDEYTDTOTALS), Globals.BeginDate, Globals.EndDate, test, Globals.GetParmValue(WhereCriteria.TXTFYSTARTMTH));

            SortBy = (DataTypes.SortBy)Convert.ToInt32(Globals.GetParmValue(WhereCriteria.SORTBY));

            lTravPgBrk = dataHelper.IsPageBreakHomeCtry(Globals.IsParmValueOn(WhereCriteria.CBPGBRKHOMECTRY), GroupBy);

            IsGraphOutput = dataHelper.IsGraphReportOutput(Globals.GetParmValue(WhereCriteria.OUTPUTTYPE));
            IsValCarr = dataHelper.IsValCarr(GroupBy, IsGraphOutput);
            IsCtryCode = dataHelper.IsCtryCode(GroupBy, IsGraphOutput);
            var howMany = Globals.GetParmValue(WhereCriteria.HOWMANY) == "" ? "0" : Globals.GetParmValue(WhereCriteria.HOWMANY);
            nHowMany = dataHelper.HowManyRecords(howMany, IsGraphOutput, GroupBy, SortBy);

            lcYTDSort = dataHelper.IsYTDSort(Globals.IsParmValueOn(WhereCriteria.CBUSEYTDNBRS), lYTDOption) ;
  
            OrderBy = dataHelper.GetOrderBy(lcYTDSort, SortBy);
            Sort = dataHelper.GetSort(Globals.GetParmValue(WhereCriteria.RBSORTDESCASC));

            IsSecondRange = false;

            if (dBegDate2.HasValue && dEndDate2.HasValue) IsSecondRange = true;

            CrystalReportName = dataHelper.GetCrystalReportName(GroupBy, IsSecondRange, IsGraphOutput);
        }

        private ReportDocument SetPdfReportSource(string rptFilePath)
        {
            var reportSource = new ReportDocument();
            reportSource.Load(rptFilePath);

            reportSource.SetDataSource(FinalDataList);
            Globals.WhereText = WhereText + cWhereExtraTxt;

            if (GroupBy != DataTypes.GroupBy.VALIDATING_CARRIER_ONLY) reportSource.Subreports[0].SetDataSource(SubReportDataList);

            var missing = CrystalFunctions.SetupCrystalReport(reportSource, Globals);

            SetReportParameters(CrystalReportName, reportSource);
            return reportSource;
        }

        private ReportDocument SetGraphReportSource(string rptFilePath)
        {
            var reportSource = new ReportDocument();
            reportSource.Load(rptFilePath);
            
            reportSource.SetDataSource(GraphFinalData);
            Globals.WhereText = WhereText + cWhereExtraTxt;

            CrystalFunctions.SetupCrystalReport(reportSource, Globals);
            //set the parameters

            reportSource.SetParameterValue("cCatDesc", IsCtryCode ? "Home Country" : "Carrier");
            reportSource.SetParameterValue("cGrColHdr1", cGrColHdr1);

            if (IsSecondRange) reportSource.SetParameterValue("cGrColHdr2", cGrColHdr2);

            reportSource.SetParameterValue("cGrDataType", graphDataHelper.GetGraphDataType(SortBy));
            reportSource.SetParameterValue("cSubtitle", graphDataHelper.GetGraphTitle(SortBy));

            return reportSource;
        }

        private void SetReportParameters(string report, ReportDocument ReportSource)
        {
            switch (report)
            {                
                case "ibTopValCarr2":
                    ReportSource.SetParameterValue("cSortBy", SortBy);
                    ReportSource.SetParameterValue("nTotCnt", nTotCnt);
                    ReportSource.SetParameterValue("nTotChg", nTotChg);
                    ReportSource.SetParameterValue("nTotCnt2", nTotCnt2);
                    ReportSource.SetParameterValue("nTotChg2", nTotChg2);
                    ReportSource.SetParameterValue("cDateDesc", cDateDesc1);
                    ReportSource.SetParameterValue("cDateDesc2", cDateDesc2);
                    break;
                case "ibTopValCarr2B":
                    ReportSource.SetParameterValue("cSortBy", SortBy);
                    ReportSource.SetParameterValue("nTotCnt", nTotCnt);
                    ReportSource.SetParameterValue("nTotChg", nTotChg);
                    ReportSource.SetParameterValue("nTotCnt2", nTotCnt2);
                    ReportSource.SetParameterValue("nTotChg2", nTotChg2);
                    ReportSource.SetParameterValue("lTravPgBrk",lTravPgBrk);
                    ReportSource.SetParameterValue("cDateDesc", cDateDesc1);
                    ReportSource.SetParameterValue("cDateDesc2",cDateDesc2);
                    break;
                case "ibTopValCarr2C":
                    ReportSource.SetParameterValue("cSortBy", SortBy);
                    ReportSource.SetParameterValue("nTotCnt", nTotCnt);
                    ReportSource.SetParameterValue("nTotChg", nTotChg);
                    ReportSource.SetParameterValue("nTotCnt2", nTotCnt2);
                    ReportSource.SetParameterValue("nTotChg2", nTotChg2);
                    ReportSource.SetParameterValue("cDateDesc", cDateDesc1);
                    ReportSource.SetParameterValue("cDateDesc2", cDateDesc2);
                    break;
                default:
                    ReportSource.SetParameterValue("cSortBy", SortBy);
                    ReportSource.SetParameterValue("nTotCnt", nTotCnt);
                    ReportSource.SetParameterValue("nTotChg", nTotChg);
                    break;
            }
        }
     


        private bool SetTotalValues(List<FinalData> finalDataList)
        {
            //get total before grouping
            if (finalDataList.Count == 0) return false;
            nTotCnt = finalDataList.Sum(x => x.Trips);
            nTotChg = finalDataList.Sum(x => x.Amt);
            nTotCnt2 = finalDataList.Sum(x => x.Trips2);
            nTotChg2 = finalDataList.Sum(x => x.Amt2);
            return true;
        }                 

    }
}
