using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.ServiceFeeDetailByTransaction;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Utilities.ClientData;
using System.Linq;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.ServiceFeeDetailByTransaction
{
    public class SvcFeeDetTran : ReportRunner<RawData, FinalData>
    {
        private readonly WhereClauseWithAdvanceParamsHandler _whereClauseWithAdvanceParamsHandler = new WhereClauseWithAdvanceParamsHandler();
        private readonly SvcFeeDetailByTransactionSqlCreator _creator = new SvcFeeDetailByTransactionSqlCreator();
        private bool IsReservationReport = false;

        public bool AccountBreak { get; set; }
        public SvcFeeDetTran()
        {
            CrystalReportName = "ibSvcFeeDetTran";
        }

        #region ReportRunner functions

        public override bool InitialChecks()
        {

            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            Globals.SetParmValue(WhereCriteria.PREPOST, "2");

            /* 09/03/2013 - the reporting travet does not have the "DATERANGE" criteria element, and there
            *  is no such thing as a "hidden varible" like we provide via the cfm/http criteria page.
            * Here we for the DATERANGE value in the aCrit array and if not found, set it to "8" - TRAN DATE
            */
            if (string.IsNullOrEmpty(Globals.GetParmValue(WhereCriteria.DATERANGE)))
                Globals.SetParmValue(WhereCriteria.DATERANGE, "8");

            AccountBreak = Globals.User.AccountBreak;

            return true;
        }

        public override bool GetRawData()
        {
            var server = Globals.AgencyInformation.ServerName;
            var db = Globals.AgencyInformation.DatabaseName;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(new iBankClientQueryable(server, db), Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: false, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false))
                return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var rawDataSql = _creator.CreateRawDataSql(BuildWhere);
            RawDataList = RetrieveRawData<RawData>(rawDataSql, IsReservationReport, false).ToList();

            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            //Check for no data
            if (!DataExists(RawDataList)) return false;

            return true;
        }


        public override bool ProcessData()
        {
            /*Go after the adv criteria again, but this time EXCLUDE SVCFEE criteria.
             * Remove them from toForm.aAdvCrit*/

            Globals.AdvancedParameters.Parameters.RemoveAll(
                s => s.AdvancedFieldName == "TAX1" || s.AdvancedFieldName == "TAX2" ||
                s.AdvancedFieldName == "TAX3" || s.AdvancedFieldName == "TAX4" ||
                s.AdvancedFieldName == "DESCRIPT" || s.AdvancedFieldName == "MCO" ||
                s.AdvancedFieldName == "SCARDNUM" || s.AdvancedFieldName == "VENDORCODE" ||
                s.AdvancedFieldName == "TRANDATE" || s.AdvancedFieldName == "SVCFEE" || s.AdvancedFieldName == "MONEYTYP");

            var serviceFeeAdvancedWhereClause = string.Empty;
            BuildWhere.AddAdvancedClauses(ref serviceFeeAdvancedWhereClause);

            var originalDateBegin = Globals.BeginDate;
            var originalWhereText = Globals.WhereText;

            Globals.BeginDate = Globals.BeginDate.Value.AddDays(-30); //WE'LL LOOK BACK 30 DAYS FOR SVC FEES AFTER THE FACT.

            BuildWhere.AddSecurityChecks();

            var sql = _creator.CreateProcessDataSql(BuildWhere, serviceFeeAdvancedWhereClause);

            //Get the route itineraries
            var retriever = new DataRetriever(ClientStore.ClientQueryDb);
            var routerItinerariesesList = retriever.GetData<RouteItineraries>(sql, BuildWhere, false, false, IsReservationReport, includeAllLegs: true, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();

            //Create lookup table for route itineraries
            var routeItineraries = SharedProcedures.GetRouteItinerary(routerItinerariesesList, true);
            var server = Globals.AgencyInformation.ServerName;
            var db = Globals.AgencyInformation.DatabaseName;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(new iBankClientQueryable(server, db), Globals.Agency);

            FinalDataList = new SvcFeeDetailByTransactionFinalData().GetFinalDataList(RawDataList, Globals, AccountBreak, clientFunctions,
                getAllMasterAccountsQuery, routeItineraries);

            //Reset the values to original values
            Globals.WhereText = originalWhereText;
            Globals.BeginDate = originalDateBegin;

            //Check for no data
            if (!FinalDataList.Any())
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_NoData;
                return false;
            }

            PerformCurrencyConversion(FinalDataList);

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFieldList = new SvcFeeDettailByTransaction().GetExportFields(AccountBreak);

                    if (Globals.OutputFormat == DestinationSwitch.Xls)
                        ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals);
                    else
                        ExportHelper.ConvertToCsv(FinalDataList, exportFieldList, Globals);
                    break;
                default: //Generate a PDF file
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;
                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);
                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    ReportSource.SetParameterValue("ILOGGEN1", !Globals.IsParmValueOn(WhereCriteria.CBSUPPSUBTOTS));

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }

        #endregion
    }
}
