using System;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.CreditCardDetail;
using Domain.Orm.iBankClientQueries;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.CreditCardDetail
{
    public class CreditCardDetail : ReportRunner<RawData,FinalData>
    {

        public CreditCardDetail()
        {
            CrystalReportName = "ibCCDetail";
        }

        public override bool InitialChecks()
        {
            if (!IsDateRangeValid()) return false;
            if (!IsOnlineReport()) return false;
            if (!Globals.ParmHasValue(WhereCriteria.DDCREDCARDCOMP))
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = "You must select a credit card company for this report.";
                return false;
            }

            return true;
        }

        public override bool GetRawData()
        {
            Globals.SetParmValue(WhereCriteria.PREPOST, "2");
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere:true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: false, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            // this report uses the ccTrans table which does not have a reckey. Therefore, it is important to set the includeAllLegs = false. Because
            // setting it to true sets makes it so the code check for reckey values
            RawDataList = RetrieveRawData<RawData>(SqlBuilder.CreateScript(Globals, BuildWhere.WhereClauseFull, BuildWhere, false), false, addFieldsFromLegsTable: false, includeAllLegs: false, checkForDuplicatesAndRemoveThem: false, handleAdvanceParamsAtReportLevelOnly: true).ToList();
            //RawDataList = ClientDataRetrieval.GetOpenQueryData<RawData>(SqlBuilder.GetSql(Globals, BuildWhere.WhereClauseFull), Globals, BuildWhere.Parameters).ToList();
            
            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            return true;
        }

        public override bool ProcessData()
        {
            FinalDataList = RawDataList
                .OrderBy(s => s.CardType)
                .ThenBy(s => s.CardNum)
                .ThenBy(s => s.PostDate)
                .ThenBy(s => s.RefNbr)
                .Select(s => new FinalData
                {
                    Cardnum = (s.CardType.Trim() + s.CardNum.Trim()).PadRight(18),
                    Refnbr = s.RefNbr,
                    Postdate = s.PostDate ?? DateTime.MinValue,
                    Trandate = s.TranDate ?? DateTime.MinValue,
                    Purchtype = CreditCardDetailHelpers.LookupPurchaseType(s.RecType),
                    Merchname = s.MerchName,
                    Merchaddr1 = s.MerchAddr1,
                    Merchcity = s.MerchCity,
                    Merchstate = s.MerchState,
                    Merchsic = s.MerchState,
                    Transamt = s.TransAmt,
                    Taxamt = s.TaxAmt,

                })
            .ToList();
            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, Globals);
                    }
                    break;
                default:
                    CreateReport();
                    break;
            }

            return true;
        }

        private void CreateReport()
        {
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            ReportSource.SetParameterValue("cDateDesc", CreditCardDetailHelpers.BuildDateDesc(Globals.BuildDateDesc(), Globals.GetParmValue(WhereCriteria.DDCREDCARDCOMP)));
            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }

    }
}
