using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Orm.iBankAdminQueries;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.BroadcastDetails;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.BroadcastDetails
{
    public class BroadcastDetails : ReportRunner<RawData, FinalData>
    {
        public BroadcastDetails()
        {
            CrystalReportName = "ibBcDetails";
        }

        public override bool InitialChecks()
        {
            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            RawDataList = RetrieveRawData<RawData>(SqlBuilder.CreateScript(Globals), false, addFieldsFromLegsTable: false, includeAllLegs: true, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();
            //RawDataList = ClientDataRetrieval.GetOpenQueryData<RawData>(SqlBuilder.GetSql(Globals), Globals, BuildWhere.Parameters).ToList();
            return true;
        }

        public override bool ProcessData()
        {
            var processes = new GetAllReportTypesQuery(MasterStore.MastersQueryDb).ExecuteQuery();
            var users = new GetAllUsersByAgencyQuery(ClientStore.ClientQueryDb, Globals.Agency).ExecuteQuery();
            var userReports = new GetAllUserReportsByAgencyQuery(ClientStore.ClientQueryDb, Globals.Agency).ExecuteQuery();
            var savedReports = new GetAllSavedReportsByAgencyQuery(ClientStore.ClientQueryDb, Globals.Agency).ExecuteQuery();
            var daysOfWeek = new[] {"SUNDAY","MONDAY","TUESDAY","WEDNESDAY","THURSDAY","FRIDAY","SATURDAY"};

            foreach (var row in RawDataList)
            {
                var newRow = new FinalData();
                Mapper.Map(row, newRow);
                newRow.Bcsndraddr = row.Bcsenderemail;
                newRow.Bcsndrnam = row.Bcsendername;
                newRow.Savrptnum = row.Savedrptnum;

                var process = processes.FirstOrDefault(s => s.ProcessKey == row.Processkey);
                if (process != null)
                {
                    newRow.Rptname = process.Caption;
                    newRow.Rpttype = "Standard Report:";
                }

                if (row.Udrkey != 0)
                {
                    var userReport = userReports.FirstOrDefault(s => s.reportkey == row.Udrkey);
                    if (userReport != null)
                    {
                        newRow.Rptname = userReport.crname.Trim();
                        newRow.Rpttype = "User Defined Report:";
                    }
                }

                if (row.Savedrptnum != 0)
                {
                    var savedReport = savedReports.FirstOrDefault(s => s.recordnum == row.Savedrptnum);
                    if (savedReport != null)
                    {
                        newRow.Rptname = savedReport.userrptnam.Trim();
                        newRow.Rpttype = "Saved Report:";
                    }
                }

                newRow.Dtypename = BroadcastDetailsHelper.GetDateTypeName(row.Datetype, row.Processkey);

                if (row.Rptusernum == 0) newRow.Rptusernum = row.UserNumber;

                var user = users.FirstOrDefault(s => s.UserNumber == newRow.Rptusernum);
                if (user != null) newRow.Otheruser = user.lastname + ", " + user.firstname;

                newRow.Schedlname = BroadcastDetailsHelper.GetScheduleName(row.Prevhist, row.Reportdays);

                newRow.Freqname = BroadcastDetailsHelper.GetFrequencyName(row.Weekmonth);

                if (row.Prevhist == 31 || (row.Prevhist != 24 && row.Prevhist != 32 && row.Prevhist != 35 &&
                     new List<int> {1, 4, 5, 6}.Contains(row.Weekmonth)))
                {
                    newRow.Rundayinfo = "; RUN REPORT ON DAY " +row.Monthrun + ". MONTH STARTS ON DAY " + row.Monthstart;
                }
                if (row.Weekmonth == 2 || row.Prevhist == 32 || row.Prevhist == 35)
                {
                    var runDate = row.Weekrun >= 1 && row.Weekrun <= 7 ? daysOfWeek[row.Weekrun - 1] : string.Empty;
                    var weekstartDate = row.Weekstart >= 1 && row.Weekstart <= 7 ? daysOfWeek[row.Weekstart - 1] : string.Empty;
                    newRow.Rundayinfo = "; RUN REPORT ON " + runDate + ". WEEK STARTS ON " + weekstartDate;
                }

                if (row.Holdrun.Equals("H")) newRow.Hldruninfo = "HOLD THE REPORT";
                if (row.Nodataoptn) newRow.Nodatainfo = "DO NOT SEND EMAIL WHEN NO DATA FOR ALL REPORTS";

                FinalDataList.Add(newRow);
            }

            return true;
        }
       
        public override bool GenerateReport()
        {
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;
            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);
            ReportSource.SetDataSource(FinalDataList);
            Globals.ReportTitle = "iBank Broadcast Report Overview";
            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            ReportSource.SetParameterValue("CDATEDESC", string.Empty);
            CrystalFunctions.CreatePdf(ReportSource, Globals);

            return true;
        }
    }
}
