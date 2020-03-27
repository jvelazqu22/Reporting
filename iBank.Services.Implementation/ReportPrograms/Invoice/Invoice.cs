using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.InvoiceReport;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.Invoice
{
    public class Invoice : ReportRunner<RawData, FinalData>
    {
        private List<LegRawData> _legDataList = new List<LegRawData>();
        private List<CarRawData> _carDataList = new List<CarRawData>();
        private List<HotelRawData> _hotelDataList = new List<HotelRawData>();
        private List<SvcFeeRawData> _svcFeeDataList = new List<SvcFeeRawData>();
        private List<SubReportData> _subReportData = new List<SubReportData>();

        private Udid _udidOne = new Udid();
        private Udid _udidTwo = new Udid();
        private string _udidOneLabel;
        private string _udidTwoLabel;

        private string _originalWhere;
        private string _acceptableTransactionTypes;

        public Invoice()
        {
            CrystalReportName = "ibInvoice";
        }

        public override bool InitialChecks()
        {
            if (!IsDateRangeValid()) return false;

            if (!Globals.BeginDate.HasValue && Globals.EndDate.HasValue) Globals.BeginDate = Globals.EndDate;

            if (Globals.BeginDate.HasValue && !Globals.EndDate.HasValue) Globals.EndDate = Globals.BeginDate;

            //**IF INVOICE NUMBER, TICKET, OR RECORD LOCATOR **
            //**ENTERED, THEN YOU DO NOT NEED A DATE RANGE.  **
            if (!Globals.ParmHasValue(WhereCriteria.INVOICE) && !Globals.ParmHasValue(WhereCriteria.TICKET) && !Globals.ParmHasValue(WhereCriteria.RECLOC))
            {
                if (!Globals.EndDate.HasValue || !Globals.BeginDate.HasValue || Globals.BeginDate.Value > Globals.EndDate.Value)
                {
                    Globals.ReportInformation.ReturnCode = 2;
                    Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_DateRange;
                    return false;
                }
            }

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        { 
            //* CALL BLD WHERE DIFFERENTLY DEPENDING IF WE HAVE A DATE RANGE
            var buildDateCondition = Globals.BeginDate.HasValue || Globals.EndDate.HasValue;
            
            //history only
            Globals.SetParmValue(WhereCriteria.PREPOST, "2");
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: false, buildDateWhere: buildDateCondition, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            _originalWhere = BuildWhere.WhereClauseFull;
            var whereX = BuildWhere.WhereClauseFull.Replace("T1.trantype != 'V'", "T1.trantype != 'Z'").Replace("T1.trantype = 'I'", "T1.trantype != 'Z'").Replace("T1.trantype = 'C'", "T1.trantype != 'Z'");

            // This code fixes a bug when searching using advance parameters by ticket #
            foreach (var param in BuildWhere.SqlParameters)
            {
                if (param.ParameterName.Equals("t1TranType1") && param.Value.Equals("V")) param.Value = "Z";
            }

            var sqlProcessor = new InvoiceSqlProcessor();
            var keysAndLocatorsSql = sqlProcessor.CreateKeysAndLocatorsSqlScript(BuildWhere, whereX, true);
            var keysAndLocators = new List<KeysAndLocators>();
            keysAndLocators = RetrieveRawData<KeysAndLocators>(keysAndLocatorsSql, false, addFieldsFromLegsTable: false, includeAllLegs: true, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();

            if (BuildWhere.HasRoutingCriteria)
            {
                keysAndLocators = GlobalCalc.IsAppliedToLegLevelData() ? BuildWhere.ApplyWhereRoute(keysAndLocators, true) : BuildWhere.ApplyWhereRoute(keysAndLocators, false);
                if (!DataExists(keysAndLocators)) return false;
            }

            keysAndLocators = keysAndLocators.Distinct().ToList();
            if (!IsUnderOfflineThreshold(keysAndLocators)) return false;

            var whereClause = "T1.reckey in (" + string.Join(",", keysAndLocators.Select(s => s.RecKey)) + ")";
         
            whereClause = !keysAndLocators.Any() ? "T1.reckey = -1 and T1.agency = '" + Globals.Agency + "'" : whereClause;
            
            var whereClause9 = !keysAndLocators.Any() ? "T1.reckey = -1 and T1.agency = '" + Globals.Agency + "'" : whereClause;

            var sql = sqlProcessor.CreateSqlScript(BuildWhere, whereClause, true);

            RawDataList = RetrieveRawData<RawData>(sql, isReservationReport: false, addFieldsFromLegsTable: false, includeAllLegs: true, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();
            
            var advHandler = new AdvancedCriteriaHandler<RawData>();
            
            RawDataList = advHandler.GetData(sql, BuildWhere, addFieldsFromLegsTable: false, includeAllLegs: false, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true);
            
            if (!DataExists(RawDataList)) return false;

            var dataRetriever = new InvoiceDataRetriever();
            dataRetriever.Globals = Globals;
            dataRetriever.IsReservation = false;

            _legDataList = dataRetriever.GetLegData(whereClause, Globals, BuildWhere, checkForDuplicatesAndRemoveThem: true);
            
            SetAcceptableTransactionTypes();

            _carDataList = dataRetriever.GetCarData(whereClause9, Globals, BuildWhere, _acceptableTransactionTypes, checkForDuplicatesAndRemoveThem: true);
            
            _hotelDataList = dataRetriever.GetHotelData(whereClause9, Globals, BuildWhere, _acceptableTransactionTypes, checkForDuplicatesAndRemoveThem: true);

            _svcFeeDataList = dataRetriever.GetServiceFeeData(whereClause, Globals, BuildWhere, _originalWhere, checkForDuplicatesAndRemoveThem: true);
            
            _udidOne = GetUdid(dataRetriever, WhereCriteria.UDIDONRPT1, WhereCriteria.UDIDLBL1, whereX);
            _udidTwo = GetUdid(dataRetriever, WhereCriteria.UDIDONRPT2, WhereCriteria.UDIDLBL2, whereX);
            _udidOneLabel = _udidOne.UdidText;
            _udidTwoLabel = _udidTwo.UdidText;

            PerformCurrencyConversion(_carDataList);
            PerformCurrencyConversion(_hotelDataList);
            PerformCurrencyConversion(RawDataList);

            return true;
        }

        private void SetAcceptableTransactionTypes()
        {
            switch (Globals.GetParmValue(WhereCriteria.INVCRED).PadRight(4).Left(4))
            {
                case "INVO":
                    _acceptableTransactionTypes = "'I'";
                    break;
                case "CRED":
                    _acceptableTransactionTypes = "'C'";
                    break;
                default:
                    _acceptableTransactionTypes = "'I','C'";
                    break;
            }
        }
         
        private Udid GetUdid(InvoiceDataRetriever dataRetriever, WhereCriteria udidNumberCrit, WhereCriteria udidTextCrit, string whereX)
        {
            var udidNumber = Globals.GetParmValue(udidNumberCrit).TryIntParse(-1);
            var udidLabel = Globals.GetParmValue(udidTextCrit);
            return dataRetriever.GetUdid(whereX, Globals, BuildWhere, udidNumber, udidLabel);
        }

        public override bool ProcessData()
        {
            var sources = new InvoiceDataSources(_legDataList, _hotelDataList, _carDataList, RawDataList, _svcFeeDataList, _udidOne, _udidTwo);
            var mapper = new FinalDataMapper();
            var reportSources = mapper.MapToFinalData(Globals, sources, _acceptableTransactionTypes, ClientStore, MasterStore);
            _subReportData = reportSources.SubReportData;
            
            FinalDataList = mapper.SortFinalDataList(Globals.GetParmValue(WhereCriteria.SORTBY), reportSources.FinalData);

            if (!DataExists(FinalDataList)) return false;

            //fix error when linking main report to subreport when it's blank 
            if (_subReportData.Count == 0) SetDummySubReportData();

            return true;
        }

        public override bool GenerateReport()
        {
            if (Globals.ReportTitle.EqualsIgnoreCase("ELECTRONIC INVOICE - COMBINED")) Globals.ReportTitle = "[NONE]";

            //this report can only use the pdf option
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);
            ReportSource.Subreports[0].SetDataSource(_subReportData);
            
            SetReportParameters();
            CrystalFunctions.CreatePdf(ReportSource, Globals);

            return true;
        }

        private void SetReportParameters()
        {
            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            var agencyInfo = Globals.AgencyInformation;
            ReportSource.SetParameterValue("cAgencyName", agencyInfo.AgencyName);
            ReportSource.SetParameterValue("cAgcyAddr1", agencyInfo.Address1);
            ReportSource.SetParameterValue("cAgcyAddr2", agencyInfo.Address2);
            ReportSource.SetParameterValue("cAgcyAddr3", agencyInfo.Address3);
            ReportSource.SetParameterValue("cAgcyAddr4", agencyInfo.Address4);
            ReportSource.SetParameterValue("cUdidLbl1", _udidOneLabel);
            ReportSource.SetParameterValue("cUdidLbl2", _udidTwoLabel);
        }

        private void SetDummySubReportData()
        {
            _subReportData.Add(new SubReportData
            {
                Reckey = 0,
                Charge = 0,
                Taxname = "",
            });
        }
    }
}
