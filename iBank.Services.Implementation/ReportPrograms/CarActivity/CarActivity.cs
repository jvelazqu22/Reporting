using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.CarActivityReport;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using System;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.CarActivity
{
    public class CarActivity : ReportRunner<RawData, FinalData>
    {
        private DateTime _firstSunday;

        public CarActivity()
        {
            CrystalReportName = "ibCarActivity";
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
            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: true, buildHotelWhere: false, 
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

            var udidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1);
            var sqlScript = SqlBuilder.GetSql(udidNumber, GlobalCalc.IsReservationReport(), BuildWhere.WhereClauseFull);
            
            RawDataList = RetrieveRawData<RawData>(sqlScript, GlobalCalc.IsReservationReport(), false, false).ToList();

            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            PerformCurrencyConversion(RawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            var userBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);

            _firstSunday = Globals.BeginDate.Value;

            while (_firstSunday.DayOfWeek != DayOfWeek.Sunday)
                _firstSunday = _firstSunday.AddDays(-1);

            FinalDataList = new CarActivityFinalData().GetFinalData(_firstSunday, Globals, ClientStore.ClientQueryDb, RawDataList, clientFunctions, userBreaks);

            return true;
        }

        public override bool GenerateReport()
        {
            var carActivityHelper = new CarActivityHelper();
            var processId = Globals.GetParmValue(WhereCriteria.PROCESSID);
            CrystalReportName = carActivityHelper.GetReportName(processId);

            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = carActivityHelper.GetExportFields(processId);

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields.ToList(), Globals);
                    else
                        ExportHelper.ListToXlsx(FinalDataList, exportFields.ToList(), Globals);
                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    if (Globals.ProcessKey == 72)
                        ReportSource.SetParameterValue("dSunDate", _firstSunday);

                    // this could be replace with a simple label in the crystal report since there is not translation 
                    // for the xPickupDate in the car activity report. If there was, this parameter would be auto-populated
                    // in the CrystalFunctions.SetParameters method.
                    if (Globals.ProcessKey == 76)
                        ReportSource.SetParameterValue("xPickupDate", "Pick-up<br>Date");
                    
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }

    }
}
