using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.CCReconReport;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.SpecifyUdid;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.CCRecon
{
    public class CcRecon : ReportRunner<RawData, FinalData>
    {
        private readonly CcReconCalculations _calc = new CcReconCalculations();

        private readonly CcReconSqlCreator _creator = new CcReconSqlCreator();

        private readonly CcReconDataProcessor _processor = new CcReconDataProcessor();

        private readonly CcReconDataRetriever _retriever = new CcReconDataRetriever();

        private IList<ServiceFee> _svcFees = new List<ServiceFee>();

        private bool _useUserBrks;

        public bool CreditCardReconcile { get; set; }
        public string CreditCardNumber { get; set; }
        public string CreditCardCompany { get; set; }

        public UserBreaks UserBreaks { get; set; }
        private bool DisplayBreaks { get; set; }
        private List<int> UdidNumber { get; set; }
        private List<string> UdidLabel { get; set; }

        public CcRecon() { }

        public override bool InitialChecks()
        {
            SetProperties();

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            if (CreditCardReconcile && (string.IsNullOrEmpty(CreditCardCompany) || string.IsNullOrEmpty(CreditCardNumber)))
            {
                const string defaultMessage = "In order to run this report with the Reconcile Report to Credit Card Data option, you must select a credit card company and specify a credit card number.";
                var errMsg = LookupFunctions.LookupLanguageTranslation("xCCReconErrMsg1", defaultMessage, Globals.LanguageVariables);
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = errMsg;
                return false;
            }

            if (Globals.IsParmValueOn(WhereCriteria.CBBRKBYUSERSETTINGS) && Globals.IsParmValueOn(WhereCriteria.CBBRKSPERUSERSETTINGS))
            {
                const string defaultMessage = @"You cannot check both the 'Break by breaks ...' and the 'Only display the breaks ...' check boxes.  You may check one or the other.";
                var errMsg = LookupFunctions.LookupLanguageTranslation("xCCReconErrMsg2", defaultMessage, Globals.LanguageVariables);
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = errMsg;
                return false;
            }

            if (!IsOnlineReport()) return false;

            return true;
        }

        private void SetProperties()
        {
            Globals.SetParmValue(WhereCriteria.DATERANGE, "8");// THIS IS ALWAYS TRANSACTION DATE.
            Globals.SetParmValue(WhereCriteria.PREPOST, "2");//THIS IS ALWAYS HISTORY.

            CreditCardReconcile = Globals.IsParmValueOn(WhereCriteria.CBRECONCILETOCCDATA);
            CreditCardNumber = Globals.GetParmValue(WhereCriteria.TXTCCNUM);
            CreditCardCompany = Globals.GetParmValue(WhereCriteria.DDCREDCARDCOMP);

            DisplayBreaks = _calc.DisplayBreaks(Globals);

            CrystalReportName = _calc.GetCrystalReportName(CreditCardReconcile, DisplayBreaks);

            var udidHandler = new UdidHandler(Globals);
            udidHandler.SetUdidOnReportProperties();
            UdidNumber = udidHandler.UdidNo;
            UdidLabel = udidHandler.UdidLabel;
        }

        public override bool GetRawData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            
            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: false, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;

            BuildWhere.BuildAdvancedClauses();           

            BuildWhere.AddSecurityChecks();

            //** 03/27/2007 - whereOriginal WILL BE USED BELOW, TO GET THE "ZZ" RECORDS THAT ARE **
            //** NEEDED TO GET BREAK INFO FOR SVC FEE RECORDS FOR HOTEL/CAR-ONLY RECORDS.  **
            //** FOR "ZZ" RECORDS, WE DON'T WANT TO APPLY THE CREDIT CARD CRITERIA.        **
            var whereClause = BuildWhere.WhereClauseFull;

            if (!string.IsNullOrEmpty(CreditCardCompany))
            {
                whereClause += _creator.GetCreditCardCompanyWhereClause(CreditCardCompany);
            }
            else
            {
                whereClause += _creator.GetNoCreditCardCompanyWhereClause();
            }

            if (!string.IsNullOrEmpty(CreditCardNumber))
            {
                whereClause += _creator.GetCreditCardNumberSql(CreditCardNumber);
                Globals.WhereText += _creator.GetCreditCardWhereText(CreditCardNumber, string.IsNullOrEmpty(Globals.WhereText));
            }

            var rawSql = _creator.GetRawDataSql(BuildWhere, _creator.ReplaceTrandateWithInvoiceDate(whereClause), Globals.AdvancedParameters.Parameters);
            RawDataList = RetrieveRawData<RawData>(rawSql, false, false).ToList();

            //_tripCcRecords = _retriever.GetTripCcRecs(_creator.ReplaceTrandateWithInvoiceDate(whereClause), Globals, ClientStore.ClientQueryDb, BuildWhere);

            //if (!DataExists(RawDataList)) return false; //could have service fee only
            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            //Get the Service Fee records
            var svcFeeSql = _creator.GetServiceFeeSql(whereClause);
            _svcFees = RetrieveRawData<ServiceFee>(svcFeeSql, false, false).ToList();

            return true;
        }

        public override bool ProcessData()
        {
            _useUserBrks = Globals.IsParmValueOn(WhereCriteria.CBBRKBYUSERSETTINGS);
            var useAcctBrks = Globals.User.AccountBreak;

            UserBreaks = _useUserBrks ? SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks) :
                Globals.IsParmValueOn(WhereCriteria.CBBRKSPERUSERSETTINGS)
                    ? SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks) : new UserBreaks();
            var whereClause = _creator.ReplaceTrandateWithInvoiceDate(BuildWhere.WhereClauseFull);

            if (!_useUserBrks)
            {
                useAcctBrks = false;
            }
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            var advanceCcFieldFound = BuildWhere.AdvancedParameterQueryTableRefList.Any(w => w.AdvancedQuerySnip.ToUpper().Contains("SFCARDNUM"));

            var rawDataToUseForRouteItinerary = advanceCcFieldFound
                ? _retriever.GetTripCcRecs<RawData>(_creator.ReplaceTrandateWithInvoiceDate(whereClause), Globals, BuildWhere, true)
                : RawDataList;

            var routeItineraries = SharedProcedures.GetRouteItinerary(rawDataToUseForRouteItinerary, true);

            FinalDataList = _processor.MapRawToFinalData(RawDataList, routeItineraries, useAcctBrks, clientFunctions,
                getAllMasterAccountsQuery, Globals, UserBreaks, DisplayBreaks, MasterStore).ToList();

            if (advanceCcFieldFound)
            {
                // when there is an advance "SFCARDNUM" field we need to get the cc records from the hibtrsips table in addition to any others
                var tripCcRecords = _retriever.GetTripCcRecs<FinalData>(_creator.ReplaceTrandateWithInvoiceDate(whereClause), Globals, BuildWhere, false);
                FinalDataList = new CcTripRecordsMapping().AddMissingRecordsFromTrips(tripCcRecords, FinalDataList, routeItineraries, useAcctBrks,
                    clientFunctions, getAllMasterAccountsQuery, Globals, UserBreaks, DisplayBreaks, MasterStore)
                    .ToList();
            }


            //var breakRecords = _retriever.GetBreakRecords(whereClause, Globals, ClientStore.ClientQueryDb, BuildWhere);

            //service fees
            var mappedSvcFeeData = _processor.MapSvcFeeDataToFinalData(FinalDataList, _svcFees, UserBreaks, DisplayBreaks, Globals, useAcctBrks, clientFunctions, getAllMasterAccountsQuery)
                                             .ToList();

            FinalDataList.AddRange(mappedSvcFeeData);

            //udids
            var udids = new List<Udid>();

            //udid number could be up to 10, we only need to retrieve udids is when at less one is configured
            if (UdidNumber.Where(x => x > 0).ToList().Count > 0)
            {
                udids = _retriever.GetUdids(UdidNumber, whereClause, Globals, BuildWhere, ClientStore.ClientQueryDb).ToList();
            }

            var sortBy = GlobalCalc.GetSortBy();
            FinalDataList = _processor.RemapFinalData(FinalDataList, useAcctBrks, udids, UdidNumber, sortBy).ToList();
            var onlyDisplayBreakByUserSetting = Globals.IsParmValueOn(WhereCriteria.CBBRKSPERUSERSETTINGS);

            FinalDataList = new CcReconSortHandler().SortFinalData(FinalDataList, sortBy, useAcctBrks, onlyDisplayBreakByUserSetting);

            if (CreditCardReconcile)
            {
                var whereTrip = _creator.GetWhereTrip(Globals.BeginDate.Value, Globals.EndDate.Value, CreditCardCompany, CreditCardNumber,
                    GlobalCalc.IncludeVoids(), _calc.IsInvoice(Globals), _calc.IsCredit(Globals));
                //TODO: Wierd select logic

                //** CC DATA - AIR. **
                var airCreditCardSql = _creator.GetAirCreditCardSql(whereTrip);
                var ccData = RetrieveRawData<CcData>(airCreditCardSql, false, false).ToList();

                ccData = _processor.MapItineraryToCreditCardData(ccData);

                //** CC DATA - SVCFEE. **
                var svcFeeCreditCardSql = _creator.GetSvcFeeCreditCardSql(whereTrip);
                var ccDataSvcFee = RetrieveRawData<CcData>(svcFeeCreditCardSql, false, false);
                ccData.AddRange(ccDataSvcFee);

                ccData = _processor.SortCreditCardData(ccData, sortBy).ToList();

                FinalDataList = _processor.MapCreditCardDataToFinalData(FinalDataList, ccData, UdidNumber, MasterStore).ToList();
            }

            if (!DataExists(FinalDataList)) return false;

            PerformCurrencyConversion(FinalDataList);

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    //var exportFieldList = new CCReconHelper().GetExportFields(Globals.User.AccountBreak, Globals.User.Break1Name, Globals.User.Break2Name,
                    //    Globals.User.Break3Name, UserBreaks, UdidNumberOne, UdidLabelOne, CrystalReportName).ToList();
                    var exportFieldList = new CCReconHelper().GetExportFields(_useUserBrks, Globals.User.Break1Name, Globals.User.Break2Name,
                        Globals.User.Break3Name, UserBreaks, UdidNumber, UdidLabel, CrystalReportName).ToList();
                    if (Globals.OutputFormat == DestinationSwitch.Xls)
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals);
                    }
                    else
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFieldList, Globals);
                    }
                    break;
                default:
                    CreatePdfReport();
                    break;
            }

            return true;
        }

        private void CreatePdfReport()
        {
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            //Create the ReportDocument object and load the .RPT File. 
            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

            if (CrystalReportName.EqualsIgnoreCase("ibCCRecon2"))
            {
                ReportSource.SetParameterValue("lLOgGen1", _calc.AreMultipleCardsOnReport(CreditCardNumber));
            }
            ReportSource.SetParameterValue("cUdidLbl1", Globals.GetParmValue(WhereCriteria.UDIDLBL1) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL1) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT1) + " text:");
            ReportSource.SetParameterValue("cUdidLbl2", Globals.GetParmValue(WhereCriteria.UDIDLBL2) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL2) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT2) + " text:");
            ReportSource.SetParameterValue("cUdidLbl3", Globals.GetParmValue(WhereCriteria.UDIDLBL3) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL3) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT3) + " text:");
            ReportSource.SetParameterValue("cUdidLbl4", Globals.GetParmValue(WhereCriteria.UDIDLBL4) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL4) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT4) + " text:");
            ReportSource.SetParameterValue("cUdidLbl5", Globals.GetParmValue(WhereCriteria.UDIDLBL5) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL5) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT5) + " text:");
            ReportSource.SetParameterValue("cUdidLbl6", Globals.GetParmValue(WhereCriteria.UDIDLBL6) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL6) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT6) + " text:");
            ReportSource.SetParameterValue("cUdidLbl7", Globals.GetParmValue(WhereCriteria.UDIDLBL7) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL7) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT7) + " text:");
            ReportSource.SetParameterValue("cUdidLbl8", Globals.GetParmValue(WhereCriteria.UDIDLBL8) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL8) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT8) + " text:");
            ReportSource.SetParameterValue("cUdidLbl9", Globals.GetParmValue(WhereCriteria.UDIDLBL9) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL9) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT9) + " text:");
            ReportSource.SetParameterValue("cUdidLbl10", Globals.GetParmValue(WhereCriteria.UDIDLBL10) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL10) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT10) + " text:");
            ReportSource.SetParameterValue("xDepartDate", "Depture Date");
            ReportSource.SetParameterValue("xArrivalDate", "Arrival Date");

            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }
    }
}
