using Domain.Helper;
using Domain.Orm.Classes;
using Domain.Orm.iBankMastersCommands;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Services.Implementation.Utilities.ReportSetup
{
    public class ReportStatusHandler
    {
        public reporthandoff RetrieveReportStatusRecord(PendingReportInformation report, IMasterDataStore masterDataStore)
        {
            var query = new GetReportStatusHandoffRecordQuery(masterDataStore.MastersQueryDb, report.ReportId, report.Agency, report.UserNumber,
                report.ColdFusionBox);
            return query.ExecuteQuery();
        }

        public bool IsRecordAvailableToRun(reporthandoff reportStatusRecord)
        {
            return reportStatusRecord != null && reportStatusRecord.parmvalue == Constants.Pending;
        }

        public void SetStatusToInProcess(IMasterDataStore masterDataStore, int serverNumber, reporthandoff reportStatusRecord)
        {
            reportStatusRecord.svrnumber = serverNumber;
            reportStatusRecord.parmvalue = Constants.InProcess;
            var cmd = new UpdateReportHandoffRecordCommand(masterDataStore.MastersCommandDb, reportStatusRecord);
            cmd.ExecuteCommand();
        }
    }
}
