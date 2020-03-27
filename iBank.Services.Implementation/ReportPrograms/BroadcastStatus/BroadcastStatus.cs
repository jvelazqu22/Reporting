using System.Linq;
using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.BroadcastStatus;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.BroadcastStatus
{
    public class BroadcastStatus : ReportRunner<RawData, FinalData>
    {
        public BroadcastStatus()
        {
            CrystalReportName = "ibbcstatus";
        }

        public override bool InitialChecks()
        {
            if (!IsDateRangeValid()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            RawDataList = RetrieveRawData<RawData>(SqlBuilder.CreateScript(Globals), false, addFieldsFromLegsTable: false, includeAllLegs: true, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();

            //var sqlString = SqlBuilder.GetSql(Globals);
            //RawDataList = ClientDataRetrieval.GetOpenQueryData<RawData>(sqlString, Globals, BuildWhere.Parameters).ToList();
            return true;
        }

        public override bool ProcessData()
        {
            var recNumber = 1;
            foreach (var row in RawDataList.OrderBy(s => s.Rundatetime))
            {
               var newRow = new FinalData
               {
                   Recnumber = recNumber++,
                   Rundatetim = row.Rundatetime
               };
                Mapper.Map(row, newRow);
                FinalDataList.Add(newRow);
                if (newRow.Emaillog.Contains("UNABLE TO SEND")) newRow.Runokay = false;
            }

            if (!DataExists(FinalDataList)) return false;
            if (!IsUnderOfflineThreshold(FinalDataList)) return false;
            return true;
        }

        public override bool GenerateReport()
        {
    
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            ReportSource.SetParameterValue("RPTTITLE", "iBank Broadcast Report Status");
            var dateDesc = "Reports run from " + Globals.BeginDate.Value.ToShortDateString() + " to " + Globals.EndDate.Value.ToShortDateString();

            if (Globals.IsParmValueOn(WhereCriteria.CBONLYBCASTERRORS)) dateDesc += " - Errors Only";

            ReportSource.SetParameterValue("CDATEDESC", dateDesc);
            CrystalFunctions.CreatePdf(ReportSource, Globals);

            return true;
        }

    }
}
