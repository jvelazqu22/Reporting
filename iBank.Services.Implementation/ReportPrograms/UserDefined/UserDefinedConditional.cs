using Domain.Helper;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public class UserDefinedConditional
    {
        public bool IsCarbonMetricHeadersOn(ReportGlobals globals, UserReportInformation userReport)
        {
            return globals.ParmHasValue(WhereCriteria.CARBONCALC) && (userReport.HasTripCarbon || userReport.HasAirCarbon) && globals.IsParmValueOn(WhereCriteria.METRIC);
        }

        public bool SuppressColumn(ReportGlobals globals, string columnName)
        {
            if (globals.OutputFormat != DestinationSwitch.XML && globals.OutputFormat != DestinationSwitch.Xls && globals.OutputFormat != DestinationSwitch.Csv)
            {
                if (columnName == "RECKEY") return true;
            }
            return false;
        }


    }
}
