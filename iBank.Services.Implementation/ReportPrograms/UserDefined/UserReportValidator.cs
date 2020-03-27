
using Domain.Helper;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public class UserReportValidator
    {
        private ReportGlobals _globals;
        private int _reportKey;
        public UserReportValidator(ReportGlobals globals, int reportKey)
        {
            _globals = globals;
            _reportKey = reportKey;
        }
        public bool IsImplementedReport(UserReportInformation userReport)
        {
            if (UserReportCheckLists.SummaryReports.Contains(userReport.ReportType.ToUpper()))
            {
                //TODO: Handle summary reports. ibCst500sl.
                _globals.ReportInformation.ReturnCode = 2;
                _globals.ReportInformation.ErrorMessage = "SUMMARY REPORTS UNDER CONSTRUCTION!, ReportKey" + _globals.GetParmValue(WhereCriteria.UDRKEY);
                return false;
            }
            return true;
        }

        public bool HasFinalColumns(List<UserReportColumnInformation> finalColumns)
        {
            if (!finalColumns.Any())
            {
                _globals.ReportInformation.ReturnCode = 2;
                _globals.ReportInformation.ErrorMessage = "Suppressing detail lines resulted in no report columns left to display. Check the layout of your report and the specification of report break fields";
                return false;
            }
            return true;
        }

        public bool IsNeededToDelay(UserReportInformation userReport)
        {
            if (userReport.Columns.Any(s => s.Name.Equals("ORIGTKTINV") || s.Name.Equals("ORIGTKTCHG") || s.Name.Equals("ORIGTKTCAR")))
            {
                var offlineMessage = "This report contains the fields from the Original Ticket Invoice. Due to the time required to process these fields, it must be run offline.";

                var clientStore = new ClientDataStore(_globals.AgencyInformation.ServerName, _globals.AgencyInformation.DatabaseName);
                var reportDelayer = new ReportDelayer(clientStore, new MasterDataStore(), _globals);
                reportDelayer.PushReportOffline(ConfigurationManager.AppSettings["ServerNumber"], offlineMessage);
                return true;
            }
            return false;
        }        

    }
}
