using System.Linq;
using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.SavedReportList;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.SavedReportList
{
    public class SavedReportList : ReportRunner<RawData,FinalData>
    {

        public SavedReportList()
        {
            CrystalReportName = "ibSavedRptList";
        }

        public override bool InitialChecks()
        {
            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            // this report uses the savedrpt1 and ibuser tables do not have a reckey. Therefore, it is important to set the includeAllLegs = false. Because
            // setting it to true sets makes it so the code check for reckey values
            RawDataList = RetrieveRawData<RawData>(SqlBuilder.CreateScript(Globals, BuildWhere.WhereClauseFull, BuildWhere, false), false, addFieldsFromLegsTable: false, includeAllLegs: false, checkForDuplicatesAndRemoveThem: false, handleAdvanceParamsAtReportLevelOnly: true).ToList();

            //RawDataList = ClientDataRetrieval.GetOpenQueryData<RawData>(SqlBuilder.GetSql(Globals), Globals, BuildWhere.Parameters).ToList();

            return true;
        }

        public override bool ProcessData()
        {
            var processKeys = new GetAllReportTypesQuery(MasterStore.MastersQueryDb).ExecuteQuery();

            FinalDataList =
                RawDataList.Where(s => !s.Userrptnam.Left(5).EqualsIgnoreCase("sysDR"))
                .Join(processKeys, r => r.Processkey, p => p.ProcessKey, (r, p) => new FinalData
                {
                    Processkey = r.Processkey,
                    Userrptnam = r.Userrptnam,
                    Lastname = r.Lastname,
                    Firstname = r.Firstname,
                    Caption = p.Caption,
                    Lastused = r.Lastused
                })
                .OrderBy(s => s.Processkey)
                .ThenBy(s => s.Userrptnam)
                .ToList();

            if (!DataExists(FinalDataList)) return false;

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                    ExportHelper.ListToXlsx(FinalDataList, Globals);
                    break;
                case DestinationSwitch.Csv:
                    ExportHelper.ConvertToCsv(FinalDataList, Globals);
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
            Globals.AccountName = Globals.CompanyName;
            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }

    }
}
