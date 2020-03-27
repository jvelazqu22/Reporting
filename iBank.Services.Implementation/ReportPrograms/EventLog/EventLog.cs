using System;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.EventLog;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.EventLog
{
    public class EventLog : ReportRunner<RawData,FinalData>
    {
        private DateTime _begDateTime = DateTime.MinValue;
        private DateTime _endDateTime = DateTime.MinValue;
        private readonly EventLogHelper _eventLogHelper = new EventLogHelper();

        public EventLog()
        {
            CrystalReportName = "ibEventLog";
        }

        public override bool InitialChecks()
        {
            if (!IsDateRangeValid()) return false;

            var acctCrit = Globals.GetParmValue(WhereCriteria.ACCT) + Globals.GetParmValue(WhereCriteria.INACCT);
            var targetOrgCrit = Globals.GetParmValue(WhereCriteria.TARGETORG) + Globals.GetParmValue(WhereCriteria.INTARGETORG);
            var targetUserCrit = Globals.GetParmValue(WhereCriteria.TARGETUSERID) + Globals.GetParmValue(WhereCriteria.INTARGETUSERID);
            var targetStyleGroupCrit = Globals.GetParmValue(WhereCriteria.TARGSTYLEGRP) + Globals.GetParmValue(WhereCriteria.INTARGSTYLEGRP);

            var critCount = (string.IsNullOrEmpty(acctCrit) ? 0 : 1)
                + (string.IsNullOrEmpty(targetOrgCrit) ? 0 : 1)
                + (string.IsNullOrEmpty(targetUserCrit) ? 0 : 1)
                + (string.IsNullOrEmpty(targetStyleGroupCrit) ? 0 : 1);

            if (critCount > 1)
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage =
                    "You can only specify one of the following: Target Organization, Target User, Target Style Group, Target Account.</strong>";
                return false;
            }

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            //Get date and time
            var begHour = Globals.GetParmValue(WhereCriteria.BEGHOUR).TryIntParse(0);
            var begMinute = Globals.GetParmValue(WhereCriteria.BEGMINUTE).TryIntParse(0);
            var isBegPm = Globals.ParmValueEquals(WhereCriteria.BEGAMPM, "2");
            var endHour = Globals.GetParmValue(WhereCriteria.ENDHOUR).TryIntParse(0);
            var endMinute = Globals.GetParmValue(WhereCriteria.ENDMINUTE).TryIntParse(0);
            var isendPm = Globals.ParmValueEquals(WhereCriteria.ENDAMPM, "2");

            _begDateTime = Globals.BeginDate.Value.Date.AddHours(begHour + (isBegPm?12:0)).AddMinutes(begMinute);
            _endDateTime = Globals.EndDate.Value.Date.AddHours(endHour + (isendPm ? 12 : 0)).AddMinutes(endMinute);

            var sql = SqlBuilder.GetSql(Globals, _begDateTime, _endDateTime);
            RawDataList = new OpenMasterQuery<RawData>(MasterStore.MastersQueryDb,sql,BuildWhere.Parameters).ExecuteQuery();
            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            return true;
        }

        public override bool ProcessData()
        {
            var orgCrits = Globals.GetParmValue(WhereCriteria.ORGANIZATION) + Globals.GetParmValue(WhereCriteria.INORGANIZATION);
            if (!string.IsNullOrEmpty(orgCrits))
            {
                var sql = "select UserNumber from ibUser where OrgKey in (" + orgCrits + ")";
                var userNumbers = ClientDataRetrieval.GetOpenQueryData<int>(sql, Globals, BuildWhere.Parameters).ToList();

                RawDataList = Globals.IsParmValueOn(WhereCriteria.NOTINORGANIZTN) 
                    ? RawDataList.Where(s => !userNumbers.Contains(s.UserNumber)).ToList() 
                    : RawDataList.Where(s => userNumbers.Contains(s.UserNumber)).ToList();
            }

            switch (Globals.GetParmValue(WhereCriteria.SORTBY))
            {
                case "2":
                    RawDataList = RawDataList.OrderBy(s => s.EventType).ThenBy(s => s.EventCode).ToList();
                    break;
                case "3":
                    RawDataList = RawDataList.OrderBy(s => s.EventTarget).ThenBy(s => s.TargetUserID).ToList();
                    break;
                default:
                    RawDataList = RawDataList.OrderBy(s => s.DateStamp).ThenBy(s => s.EventCode).ToList();
                    break;
            }
            
            FinalDataList = (Globals.ClientType == ClientType.Sharer)
                ? new EventLogProcessor().GetFinalDataUsingUsingMultipleAgencies(RawDataList, ClientStore, clientFunctions, Globals)
                : new EventLogProcessor().GetFinalDataUsingSingleAgency(RawDataList, ClientStore, clientFunctions, Globals);

            if (!DataExists(FinalDataList)) return false;

            return true;
        }

        public override bool GenerateReport()
        {
            var includeIp = Globals.IsParmValueOn(WhereCriteria.CBINCLIPADDRESS);
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, _eventLogHelper.GetExportFields(includeIp), Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, _eventLogHelper.GetExportFields(includeIp), Globals);
                    }
                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    ReportSource.SetParameterValue("LLOGGEN1", includeIp);
                    ReportSource.SetParameterValue("CDATEDESC", "Events from " + _begDateTime + " to " + _endDateTime);
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            return true;
        }

    }
}
