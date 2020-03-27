using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.ReportLog;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities.ClientData;
using FinalData = Domain.Models.ReportPrograms.ReportLog.FinalData;
using RawData = Domain.Models.ReportPrograms.ReportLog.RawData;

namespace iBank.Services.Implementation.ReportPrograms.ReportLog
{
    public class ReportLog : ReportRunner<RawData, FinalData>
    {
        private List<ReportLogCriteria> _criteria;
        private List<int> _userNumbers; 

        public ReportLog()
        {
            CrystalReportName = "ibRptLog";
            _criteria = new List<ReportLogCriteria>();
            _userNumbers = new List<int>();
        }

        public override bool InitialChecks()
        {
            if (!IsDateRangeValid()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            // this does not apply here because the ibrptlog is part of the ibankmasters database and not the demo database
            //RawDataList = RetrieveRawData<RawData>(SqlBuilder.CreateScript(Globals), false, addFieldsFromLegsTable: false, includeAllLegs: true, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();
            RawDataList = new OpenMasterQuery<RawData>(new iBankMastersQueryable(), SqlBuilder.GetReportLogSql(Globals),BuildWhere.Parameters).ExecuteQuery();
            if (Globals.ParmValueEquals(WhereCriteria.RBRPTVERSION, "2"))
            {
                _criteria = new OpenMasterQuery<ReportLogCriteria>(new iBankMastersQueryable(), SqlBuilder.GetReportLogCriteriaSql(Globals), BuildWhere.Parameters).ExecuteQuery();
            }
            if (Globals.User.AdminLevel != 1 || Globals.ParmHasValue(WhereCriteria.ORGANIZATION) || Globals.ParmHasValue(WhereCriteria.INORGANIZATION))
            {
                var sql = SqlBuilder.GetOrganizationSql(Globals);
                _userNumbers = ClientDataRetrieval.GetOpenQueryData<int>(sql, Globals, BuildWhere.Parameters).ToList();
                RawDataList = RawDataList.Where(s => _userNumbers.Contains(s.UserNumber)).ToList();
            }
            return true;
        }

        public override bool ProcessData()
        {
            var crystalReportName = CrystalReportName;
            FinalDataList = new ProcessDataHandler().GetFinalDataAndUpdateCrystalReportName(RawDataList, Globals, ref crystalReportName, _criteria, BuildWhere);
            CrystalReportName = crystalReportName;

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
            Globals.AccountName = Globals.CompanyName;
            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            ReportSource.SetParameterValue("CDATEDESC", "Report run dates from " + Globals.BeginDate.Value.ToShortDateString() + " to " + Globals.EndDate.Value.ToShortDateString());
            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }
    }
}
