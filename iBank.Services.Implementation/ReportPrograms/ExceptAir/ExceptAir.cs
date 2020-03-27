using System.Linq;

using CODE.Framework.Core.Utilities;

using CrystalDecisions.CrystalReports.Engine;

using Domain.Helper;

using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;

using Domain.Models.ReportPrograms.ExceptAirReport;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.ExceptAir
{
    public class ExceptAir : ReportRunner<RawData, FinalData>
    {
        private readonly ExceptAirCalculations _calc = new ExceptAirCalculations();

        private readonly ExceptAirDataProcessor _processor = new ExceptAirDataProcessor();

        private readonly ExceptAirSqlCreator _creator = new ExceptAirSqlCreator();
        
        public UserBreaks UserBreaks { get; set; }
        public bool AccountBreak { get; set; }
        public int UdidNumber { get; set; }
        public string ColHead1 { get; set; }
        public bool UseBaseFare { get; set; }

        public ExceptAir()
        {
        }

        public override bool InitialChecks()
        {
            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        private void SetProperties()
        {
            UdidNumber = GlobalCalc.GetUdidNumber();
            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);
            AccountBreak = Globals.User.AccountBreak;
            Globals.AccountName = Globals.CompanyName;
            UseBaseFare = GlobalCalc.UseBaseFare();

            CrystalReportName = _calc.GetCrystalReportName();
        }

        public override bool GetRawData()
        {
            SetProperties();

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var sqlScript = _creator.Create(BuildWhere.WhereClauseFull, UdidNumber, Globals.AgencyInformation.ReasonExclude, GlobalCalc.IsReservationReport());
          
            RawDataList = RetrieveRawData<RawData>(sqlScript, GlobalCalc.IsReservationReport(), false).ToList();

            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            
            PerformCurrencyConversion(RawDataList);

            if (UseBaseFare) RawDataList = _processor.ReplaceAirChargeWithBaseFare(RawDataList).ToList();

            return true;
        }

        public override bool ProcessData()
        {
            ColHead1 = _calc.GetColumnOneHeader(UseBaseFare);

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            FinalDataList = _processor.MapRawToFinalData(RawDataList, AccountBreak, clientFunctions, Globals, UserBreaks, getAllMasterAccountsQuery,
                MasterStore, ClientStore).ToList();

            if (!DataExists(FinalDataList)) return false;
            
            return true;
        }

        public override bool GenerateReport()
        {
            var zeroFields = _calc.GetZeroFields(UseBaseFare).ToList();

            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFieldList = _calc.GetExportFields(AccountBreak, UserBreaks, Globals.User.Break1Name, Globals.User.Break2Name, Globals.User.Break3Name, UseBaseFare).ToList();

                    FinalDataList = ZeroOut<FinalData>.Process(FinalDataList, zeroFields);

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
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;
                    
                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);
                    
                    ReportSource.SetDataSource(FinalDataList);
                    
                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    SetNonGlobalReportParams();
                    
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }

        private void SetNonGlobalReportParams()
        {
            ReportSource.SetParameterValue("CCOLHEAD1", ColHead1);
        }
        
    }
}
