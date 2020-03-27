using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomUdids;
using Domain.Orm.iBankClientQueries;

using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomUdids
{
    public class TopBottomUdids : ReportRunner<RawData, FinalData>
    {

        private int _udidNumber;

        public TopBottomUdids()
        {
            CrystalReportName = "ibTopUdids";
        }
        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;
            if (!IsDateRangeValid()) return false;

            _udidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1);
            if (_udidNumber < 0)
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = "You must specify a UDID # for this report.";
                return false;
            }

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            var server = Globals.AgencyInformation.ServerName;
            var db = Globals.AgencyInformation.DatabaseName;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(new iBankClientQueryable(server, db), Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: false, buildDateWhere: true,inMemory: false,isRoutingBidirectional: false,legDit: false,ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var cWhereUdid = SqlBuilder.GetUdidSql(_udidNumber, Globals);

            var sql = SqlBuilder.GetSql(GlobalCalc.IsReservationReport(),BuildWhere.WhereClauseFull + cWhereUdid);
            RawDataList = RetrieveRawData<RawData>(sql, GlobalCalc.IsReservationReport(), false).ToList();

            return true;
        }

        public override bool ProcessData()
        {
            RawDataList =
                RawDataList.GroupBy(s => s.UdidText,
                    (key, recs) => new RawData {UdidText = key, UdidCount = recs.Count()}).ToList();

            FinalDataList = TopBottomUdidsHelper.SortAndFilter(RawDataList, Globals.GetParmValue(WhereCriteria.SORTBY),
                !Globals.ParmValueEquals(WhereCriteria.RBSORTDESCASC, "2"),
                Globals.GetParmValue(WhereCriteria.HOWMANY).TryIntParse(-1));

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = new List<string> {"UdidText", "UdidCount"};
                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                    }
                    break;
                default:

                   
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    ReportSource.SetParameterValue("nTotCnt", RawDataList.Sum(s => s.UdidCount));
        


                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            return true;
        }
    }
}
