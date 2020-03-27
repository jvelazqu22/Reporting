namespace Domain.Constants
{
    #region Region - Attempt at explanation of logic

    /* Overview
        The ibbatch values prevhist, weekmonth, monthrun, weekrun, and weekstart are all numeric coded values
        that are set by various factors in the scheduling of a broadcast. These are holdover values from the FoxPro 
        original logic. They should eventually be changed to something that makes sense, but for now, here they are in all their glory.
    */

    /* ibbatch.prevhist -- this is what data the broadcast covers
        1 = Reservation
        2 = BackOffice
        3 = Reservation Previous Day
        4 = Reservation Previous Week
        5 = Reservation Previous Month
        6 = Back Office Next Week
        7 = Back Office Previous Day
        21 = Reservation Mon Wed Fri
        22 = Reservation  Sun Tue Thurs
        23 = Reservation Tues Thurs Sat
        24 = Reservation Weekly
        25 = Reservation Daily 
        31 = YTD Back Office Run Monthly
        32 = YTD Back Office Run Weekly
        33 = YTD Reservation Run Monthly
        34 = YTD Reservation Run Weekly
        35 = MTD Back Office Run Weekly
        36 = MTD Back Office Run Daily
        37 = MTD Reservation Run Weekly 
        38 = MTD Reservation Run Daily
    */

    /* ibbatch.weekmonth -- this is the frequency in which the broadcast runs
        -24 thru -59 = Daily Every x Minutes, where x is the value
        -1 thru -23 = Daily Every x Hours, where x is the value
        1 = Monthly
        2 = Weekly
        3 = Daily
        4 = Quarterly
        5 = Semi-Annual
        6 = Annual
        7 = Current Day
        31 = Daily Prior Business Day
        32 = Daily Next Business Day
     */

    // ibbatch.monthrun -- the day of the month on which monthly reports are run

    // ibbatch.monthstart -- the day of the week that is the starting date range for data in monthly report - max value is 28

    /* ibbatch.weekrun -- the day of the week on which the report is run
        1 = Sunday
        2 = Monday
        3 = Tuesday
        4 = Wednesday
        5 = Thursday
        6 = Friday
        7 = Saturday
      */

    /* ibbatch.weekstart -- the day of the week that is the starting date range for data in a report -- this value is ignored on monthly reports

        Two notes: These are not in the same numerical order as weekrun
                   If ibbatch.prevhist != 6 the numbers are all one digit,
                    but if ibbatch.prevhist == 6 those values are all two digits, which signifies to set the start date in the future rather than in the past
        0 = Offline Report

        if ibbatch.prevhist != 6 then you have 
        1 = Monday
        2 = Tuesday
        3 = Wednesday
        4 = Thursday
        5 = Friday
        6 = Saturday
        7 = Sunday

        but if ibbatch.prevhist == 6 --> Monday and Sunday already existed, so that is why they do not follow convention
        1 = Monday
        21 = Tuesday
        31 = Wednesday
        41 = Thursday
        51 = Friday
        61 = Saturday
        2 = Sunday
     */

    #endregion

    /// <summary>
    /// These values relate to ibbatch.weekmonth
    /// </summary>
    public class BroadcastFrequencyOfRun
    {
        public const int MONTHLY = 1;
        public const int WEEKLY = 2;

        public const int DAILY = 3;

        public const int QUARTERLY = 4;

        public const int SEMI_ANNUAL = 5;

        public const int ANNUAL = 6;

        public const int CURRENT_DAY = 7;

        public const int NOT_SCHEDULED = 9;

        public const int BI_WEEKLY = 11;

        public const int BI_MONTHLY = 10;

        public const int DAILY_PRIOR_BUSINESS_DAY = 31;

        public const int DAILY_NEXT_BUSINESS_DAY = 32;

        public static bool IsDailyEveryXMinutes(int frequencyOfRun)
        {
            return frequencyOfRun <= -30 && frequencyOfRun >= -59;
        }

        public static bool IsDailyEveryXHours(int frequencyOfRun)
        {
            return frequencyOfRun <= -1 && frequencyOfRun >= -23;
        }
    }

    /// <summary>
    /// These values relate to ibbatch.prevhist
    /// </summary>
    public class BroadcastSchedule
    {
        public const int RESERVATION = 1;
        public const int BACK_OFFICE = 2;

        public const int RESERVATION_PREVIOUS_DAY = 3;

        public const int RESERVATION_PREVIOUS_WEEK = 4;

        public const int RESERVATION_PREVIOUS_MONTH = 5;

        public const int BACK_OFFICE_NEXT_WEEK = 6;

        public const int BACK_OFFICE_PREVIOUS_DAY = 7;

        public const int RESERVATION_MON_WED_FRI = 21;

        public const int RESERVATION_SUN_TUE_THURS = 22;

        public const int RESERVATION_TUES_THURS_SAT = 23;

        public const int RESERVATION_WEEKLY = 24;

        public const int RESERVATION_DAILY = 25;

        public const int YTD_BACK_OFFICE_RUN_MONTHLY = 31;

        public const int YTD_BACK_OFFICE_RUN_WEEKLY = 32;

        public const int YTD_RESERVATION_RUN_MONTHLY = 33;

        public const int YTD_RESERVATION_RUN_WEEKLY = 34;

        public const int MTD_BACK_OFFICE_RUN_WEEKLY = 35;

        public const int MTD_BACK_OFFICE_RUN_DAILY = 36;

        public const int MTD_RESERVATION_RUN_WEEKLY = 37;

        public const int MTD_RESERVATION_RUN_DAILY = 38;
    }

    /// <summary>
    /// These values relate to ibbatch.weekrun
    /// </summary>
    public class DayOfWeekToRun
    {
        public const int SUNDAY = 1;

        public const int MONDAY = 2;

        public const int TUESDAY = 3;

        public const int WEDNESDAY = 4;

        public const int THURSDAY = 5;

        public const int FRIDAY = 6;

        public const int SATURDAY = 7;
    }

    public class MonthOfTheYear
    {
        public const int JANUARY = 1;

        public const int FEBRUARY = 2;

        public const int MARCH = 3;

        public const int APRIL = 4;

        public const int MAY = 5;

        public const int JUNE = 6;

        public const int JULY = 7;
        public const int AUGUST = 8;
        public const int SEPTEMBER = 9;
        public const int OCTOBER = 10;
        public const int NOVEMBER = 11;
        public const int DECEMBER = 12;
    }

    /// <summary>
    /// These classes relate to ibbatch.weekstart
    /// </summary>
    public class DayOfWeekOfStartingRangeForData
    {
        public const int MONDAY = 1;

        public static int GetTuesday(int broadcastSchedule)
        {
            return broadcastSchedule == 6 ? 21 : 2;
        }

        public static int GetWednesday(int broadcastSchedule)
        {
            return broadcastSchedule == 6 ? 31 : 3;
        }

        public static int GetThursday(int broadcastSchedule)
        {
            return broadcastSchedule == 6 ? 41 : 4;
        }

        public static int GetFriday(int broadcastSchedule)
        {
            return broadcastSchedule == 6 ? 51 : 5;
        }

        public static int GetSaturday(int broadcastSchedule)
        {
            return broadcastSchedule == 6 ? 61 : 6;
        }

        public static int GetSunday(int broadcastSchedule)
        {
            return broadcastSchedule == 6 ? 2 : 7;
        }
    }


}
