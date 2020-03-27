using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;

namespace iBank.Services.Implementation.ReportPrograms.TravAuthKpi
{
    public class TravAuthKPIDateValidator
    {
        public string MonthName { get; set; }
        public int Year { get; set; }
        public bool Validate(ReportGlobals globals)
        {
            MonthName = globals.GetParmValue(WhereCriteria.STARTMONTH);
            var _month = SharedProcedures.GetMonthNum(MonthName);
            Year = globals.GetParmValue(WhereCriteria.STARTYEAR).TryIntParse(-1);
            
            if (!_month.IsBetween(1, 12) || !Year.IsBetween(2000, 2030))
            {
                globals.ReportInformation.ReturnCode = 2;
                globals.ReportInformation.ErrorMessage = "You need to specify a month and year.";
                return false;
            }

            return true;
        }
    }
}
