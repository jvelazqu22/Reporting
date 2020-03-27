using System;

using Domain.Helper;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.SharedClasses
{
    public class TripChangesCalculations
    {
        public DateTime? GetBeginDate2(ReportGlobals globals)
        {
            var beginDate2 = globals.GetParmValue(WhereCriteria.BEGDATE3).ToDateFromiBankFormattedString();

            if (!beginDate2.HasValue)
            {
                beginDate2 = globals.GetParmValue(WhereCriteria.CHANGESTAMP).ToDateFromiBankFormattedString();
            }

            return beginDate2;
        }

        public DateTime? GetEndDate2(ReportGlobals globals)
        {
            var endDate2 = globals.GetParmValue(WhereCriteria.ENDDATE3).ToDateFromiBankFormattedString();

            if (!endDate2.HasValue)
            {
                endDate2 = globals.GetParmValue(WhereCriteria.CHANGESTAMP2).ToDateFromiBankFormattedString();
            }

            return endDate2;
        }

        public bool IsDateRangeValid(DateTime? beginDate, DateTime? endDate, ReportGlobals globals)
        {
            if (beginDate.HasValue && endDate.HasValue && !beginDate.Value.IsPriorToOrSameDay(endDate.Value))
            {
                globals.ReportInformation.ReturnCode = 2;
                globals.ReportInformation.ErrorMessage = globals.ReportMessages.RptMsg_DateRange;
                return false;
            }

            return true;
        }

        public DateTime? ReassignDate(DateTime? dateOne, DateTime? dateTwo)
        {
            if (!dateOne.HasValue && dateTwo.HasValue)
            {
                return dateTwo;
            }

            return dateOne;
        }
    }
}
