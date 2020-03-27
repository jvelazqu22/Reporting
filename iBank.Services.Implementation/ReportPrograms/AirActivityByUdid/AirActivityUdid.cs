using System.Collections.Generic;
using System.Linq;

using CODE.Framework.Core.Utilities;

using Domain.Helper;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;

using Domain.Models.ReportPrograms.AirActivityByUdidReport;
using Domain.Orm.iBankClientQueries;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.AirActivityByUdid
{
    public class AirActivityUdid : ReportRunner<RawData, FinalData>
    {
        private readonly AirActivityUdidCalculations _calc = new AirActivityUdidCalculations();

        private readonly AirActivityUdidSqlCreator _creator = new AirActivityUdidSqlCreator();

        private readonly AirActivityUdidDataProcessor _processort = new AirActivityUdidDataProcessor();
        public UserBreaks UserBreaks { get; set; }
        private bool IsReservationReport { get; set; }
        
        public AirActivityUdid()
        {
        }

        private void SetProperties()
        {
            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);
            IsReservationReport = GlobalCalc.IsReservationReport();
            CrystalReportName = _calc.GetCrystalReportName();
        }

        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSupplied()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            SetProperties();

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            //Note that we are not building the Route clause here. We'll need the in-memory version later 
            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var sql = _creator.Create(IsReservationReport, BuildWhere.WhereClauseFull);
            RawDataList = RetrieveRawData<RawData>(sql, IsReservationReport, false).ToList();

            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            
            PerformCurrencyConversion(RawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            FinalDataList = _processort.MapRawToFinalData(RawDataList, Globals.User.AccountBreak, clientFunctions, UserBreaks,
                    new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency), Globals).ToList();
            
            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFieldList = _calc.GetExportFields(Globals.User.AccountBreak, UserBreaks, Globals.User).ToList();
                    FinalDataList = ZeroOut<FinalData>.Process(FinalDataList, new List<string> { "airchg" });

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
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    var udidLbl1 = Globals.GetParmValue(WhereCriteria.UDIDLBL1);
                    if (string.IsNullOrEmpty(udidLbl1))
                    {
                        udidLbl1 = "Udid Text:";
                    }
                    if (!udidLbl1.Right(1).Equals(":"))
                    {
                        udidLbl1 += ":";
                    }
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    ReportSource.SetParameterValue("cUdidLbl1", udidLbl1);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            return true;
        }
    }
}
