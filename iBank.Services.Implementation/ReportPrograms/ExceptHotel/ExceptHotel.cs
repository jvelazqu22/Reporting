using System.Linq;

using CODE.Framework.Core.Utilities;

using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.ExceptHotelReport;
using Domain.Orm.iBankClientQueries;

using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.ExceptHotel
{
    public class ExceptHotel : ReportRunner<RawData, FinalData>
    {
        public ExceptHotel()
        {
        }

        private readonly ExceptHotelCalculations _calc = new ExceptHotelCalculations();

        private readonly ExceptHotelProcessor _processor = new ExceptHotelProcessor();

        private readonly ExceptHotelSqlCreator _creator = new ExceptHotelSqlCreator();
        public UserBreaks UserBreaks { get; set; }
        public bool AccountBreak { get; set; }

        public override bool InitialChecks()
        {
            if (!IsDateRangeValid()) return false;
            
            if (!IsUdidNumberSuppliedWithUdidText()) return false;
            
            if (!IsOnlineReport()) return false;

            return true;
        }

        private void SetProperties()
        {
            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);
            AccountBreak = Globals.User.AccountBreak;

            CrystalReportName = _calc.GetCrystalReportName();
        }

        public override bool GetRawData()
        {
            SetProperties();

            var reasList = GlobalCalc.GetReasonList();
            var reasCode = GlobalCalc.GetReasonCode();
            var reasNot = GlobalCalc.IsNotInReason();

            Globals.SetParmValue(WhereCriteria.INREASCODE, "");
            Globals.SetParmValue(WhereCriteria.REASCODE, "");
            Globals.SetParmValue(WhereCriteria.NOTINREAS, "");
            
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false,buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddAdvancedClauses();
            BuildWhere.AddSecurityChecks();
            
            var whereClause = BuildWhere.WhereClauseFull;

            if (!string.IsNullOrEmpty(reasList))
            {
                //You probably don't need to use TextList; the key is to make sure you have a comma=delimited list of strings. 
                reasList = SharedProcedures.TextList(reasList);
                //so here, you'd write something like this(note the change to reasNot above to make it a boolean):
                var isNotInList = reasNot ? " Not " : string.Empty;
                var reasNotEqual = reasNot ? " Not = " : " = ";
                whereClause += "and reascoda " + isNotInList + "in (" + reasList + ")";
                Globals.WhereText += "Exception Reason Code" + reasNotEqual + reasList + ";";
            }
            else
            {
                if (!string.IsNullOrEmpty(reasCode))
                {
                    var isNotEqual = reasNot ? " != '" : " = '";
                    var reasNotEqual = reasNot ? " Not = " : " = ";
                    whereClause += " and reascoda " + isNotEqual + reasCode + "'";
                    Globals.WhereText += "Exception Reason Code" + reasNotEqual + reasList + ";";
                }
            }
            

            var sql = _creator.Create(whereClause, GlobalCalc.GetUdidNumber(), GlobalCalc.IsReservationReport(), Globals.AgencyInformation.ReasonExclude);
            RawDataList = RetrieveRawData<RawData>(sql, GlobalCalc.IsReservationReport(), false).ToList();
            
            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold((RawDataList))) return false;

            PerformCurrencyConversion(RawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            //Process the Raw data into Final data, which is the data set we will feed into crystal (or export to CSV, XLS)
           
            if (GlobalCalc.IgnoreBreakSettings())
            {
                AccountBreak = false;
                UserBreaks.UserBreak1 = false;
                UserBreaks.UserBreak2 = false;
                UserBreaks.UserBreak3 = false;
            }
            
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            FinalDataList = _processor.MapRawToFinalData(RawDataList, AccountBreak, UserBreaks, clientFunctions, getAllMasterAccountsQuery, Globals, ClientStore, MasterStore).ToList();
            
            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFieldList = _calc.GetExportFields(AccountBreak, UserBreaks, Globals.User.Break1Name, Globals.User.Break2Name, 
                        Globals.User.Break3Name).ToList();

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

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);
                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }
    }

}