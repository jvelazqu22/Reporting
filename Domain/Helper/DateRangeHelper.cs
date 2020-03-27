using iBank.Entities.MasterEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using NodaTime.Extensions;

namespace Domain.Helper
{
    public static class DateRangeHelper
    {
        public static int GetActualDateRangeInterval(bcstque4 bcst)
        {
            var startDate = bcst.nxtdstart ?? new DateTime(1900, 1, 1);
            var endDate = bcst.nxtdend ?? new DateTime(1900, 1, 1);

            /* 
              we add 1 day to the end date so that the end of the month rolls over - 1/1/17 - 12/31/17 becomes 1/1/17 - 1/1/18, which is 12 months
                this makes the storage interval more human readable - a person would consider 1/1/17 - 12/31/17 a 12 month interval, 
                even though that is really 11 months + 31 days
            */
            endDate = GetEndDateThatIsNotAFutureDate(endDate);

            var period = Period.Between(startDate.ToLocalDateTime(), endDate.AddDays(1).ToLocalDateTime(), PeriodUnits.Months);
            return period.Months;
        }

        public static DateTime GetEndDateThatIsNotAFutureDate(DateTime endDate)
        {
            //Date range to determine if a broadcast is long-running should be PAST dates
            var now = DateTime.Now;
            var today = new DateTime(now.Year, now.Month, now.Day);

            var endDateToToday = Period.Between(today.ToLocalDateTime(), endDate.AddDays(1).ToLocalDateTime(), PeriodUnits.Days);
            if (endDateToToday.Days > 0) return today;

            return endDate;
        }

        public static int GetSpecialDateRangeInterval(bcstque4 bcst)
        {
            var startDate = bcst.spclstart ?? new DateTime(1900, 1, 1);
            var endDate = bcst.spclend ?? new DateTime(1900, 1, 1);

            /* 
              we add 1 day to the end date so that the end of the month rolls over - 1/1/17 - 12/31/17 becomes 1/1/17 - 1/1/18, which is 12 months
                this makes the storage interval more human readable - a person would consider 1/1/17 - 12/31/17 a 12 month interval, 
                even though that is really 11 months + 31 days
            */
            endDate = GetEndDateThatIsNotAFutureDate(endDate);

            var period = Period.Between(startDate.ToLocalDateTime(), endDate.AddDays(1).ToLocalDateTime(), PeriodUnits.Months);
            return period.Months;
        }
    }
}
