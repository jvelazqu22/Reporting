using System;

namespace iBank.Services.Implementation.Utilities
{
    public class DateConverter
    {
        public static DateTime? ReplaceEmptyDate(DateTime? possibleEmptyDate, DateTime? controlDate)
        {
            if (possibleEmptyDate == null && controlDate != null)
            {
                return controlDate;
            }

            return possibleEmptyDate;
        }
    }
}
