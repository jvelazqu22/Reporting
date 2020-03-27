using System.Linq;
using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;

using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedLayout;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.UserDefinedLayout
{
    public class UserDefinedLayout  : ReportRunner<RawData,FinalData>
    {

        public UserDefinedLayout()
        {
            CrystalReportName = "ibcstdatadict";
        }

        public override bool InitialChecks()
        {
            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            //not really using buildwhere, but there are some setup items in there, so calling it with all false. 
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: false, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: false, buildDateWhere: false, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false);

            var sql = SqlBuilder.GetSql(Globals);
            RawDataList = ClientDataRetrieval.GetOpenQueryData<RawData>(sql, Globals, BuildWhere.Parameters).ToList();

            return true;
        }

        public override bool ProcessData()
        {
            var cols = new OpenMasterQuery<ColumnData>(MasterStore.MastersQueryDb, "select colname, bigname from collist2",BuildWhere.Parameters).ExecuteQuery();

            FinalDataList =
                RawDataList.Join(cols, r => r.Colname.Trim(), c => c.ColName.Trim(), (r, c) =>
                {
                    var newRawData = new FinalData();
                    Mapper.Map(r, newRawData);
                    newRawData.Bigname = string.IsNullOrEmpty(c.BigName)? string.Empty: c.BigName;
                    return newRawData;
                })
                .OrderBy(s => s.Crtype)
                .ThenBy(s => s.Crname)
                .ThenBy(s => s.Reportkey)
                .ThenBy(s => s.Colorder)
                .ToList();

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:

                    var exportFieldList = UserDefinedLayoutHelper.GetExportFields();

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFieldList, Globals, "", true, $"{Globals.DateDisplay} HH:mm");
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals, true, $"{Globals.DateDisplay} HH:mm");
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
