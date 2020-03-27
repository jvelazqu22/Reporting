using Domain.Helper;

using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;

namespace iBank.Server.Utilities.ReportGlobalsSetters
{
    public class GlobalValuesSetter
    {
        public void SetGlobals(ReportGlobals globals)
        {
            globals.HstPrePref = GetReportDataTypeName(globals);
            globals.TitlePrefix = GetReportTitlePrefix(globals);

            var parametersSetter = new ReportParametersGlobalsSetter();
            parametersSetter.SetValuesFromReportParameters(globals);

            var masterGlobals = new MasterValuesGlobalsSetter(new MasterDataStore());
            masterGlobals.SetMasterDatabaseGlobals(globals);

            var clientDataStore = new ClientDataStore(globals.AgencyInformation.ServerName, globals.AgencyInformation.DatabaseName);
            var clientGlobals = new ClientValuesGlobalsSetter(clientDataStore, new MasterDataStore());
            clientGlobals.SetClientDatabaseGlobals(globals);
        }

        private static string GetReportDataTypeName(ReportGlobals globals)
        {
            var prepost = globals.GetParmValue(WhereCriteria.PREPOST);
            switch (prepost)
            {
                case "1":
                    return "Reservation Data";
                case "2":
                    return "Back Office Data";
                case "3":
                    return "Special";
                case "4":
                    return "Ticket Tracker Data";
                case "5":
                    return "Credit Card Data";
                default:
                    return globals.HstPrePref;
            }
        }

        private static string GetReportTitlePrefix(Classes.ReportGlobals globals)
        {
            if (globals.ProcessKey == 247 || globals.ProcessKey == 249)
            {
                return "AXO ";
            }

            return globals.TitlePrefix;
        }
    }
}
