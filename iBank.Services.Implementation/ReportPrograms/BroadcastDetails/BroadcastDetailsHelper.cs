namespace iBank.Services.Implementation.ReportPrograms.BroadcastDetails
{
    public static class BroadcastDetailsHelper
    {

        public static string GetScheduleName(int prevHist, int reportDays)
        {
            switch (prevHist)
            {
                case 2:
                    return "HISTORY";
                case 1:
                    return "PREVIEW";
                case 3:
                    return "PREVIEW (PREVIOUS DAY)";
                case 4:
                    return "PREVIEW (PREVIOUS WEEK)";
                case 5:
                    return "PREVIEW (PREVIOUS MONTH)";
                case 6:
                    return "HISTORY (NEXT WEEK)";
                case 21:
                    return "SPCL PREVIEW: M-W-F NEXT " + reportDays + " DAYS";
                case 22:
                    return "SPCL PREVIEW: S-T-Th NEXT " + reportDays + " DAYS";
                case 23:
                    return "SPCL PREVIEW: T-Th-S NEXT " + reportDays + " DAYS";
                case 24:
                    return "SPCL PREVIEW: WEEKLY NEXT " + reportDays + " DAYS";
                case 25:
                    return "SPCL PREVIEW: DAILY NEXT " + reportDays + " DAYS";
                case 31:
                    return "YTD HISTORY, RUN MONTHLY";
                case 32:
                    return "YTD HISTORY, RUN WEEKLY";
                case 33:
                    return "YTD PREVIEW, RUN MONTHLY";
                case 34:
                    return "YTD PREVIEW, RUN WEEKLY";
                case 35:
                    return "MTD HISTORY, RUN WEEKLY";
                case 36:
                    return "MTD HISTORY, RUN DAILY";
                case 37:
                    return "MTD PREVIEW, RUN WEEKLY";
                case 38:
                    return "MTD PREVIEW, RUN DAILY";
            }
            return string.Empty;
        }

        public static string GetFrequencyName(int weekMonth)
        {
            switch (weekMonth)
            {
                case 1:
                    return "MONTHLY";
                case 2:
                    return "WEEKLY";
                case 3:
                    return "DAILY (PREVIEW ONLY)";
                case 7:
                    return "CURRENT DAY (PREVIEW ONLY)";
                case 4:
                    return "QUARTERLY";
                case 5:
                    return "SEMI-ANNUAL";
                case 6:
                    return "ANNUAL";
            }

            return string.Empty;
        }

        public static string GetDateTypeName(int dateType, int processKey)
        {
            if (dateType == 1) return "DEPARTURE";
            if (dateType == 2) return "INVOICE";
            if (dateType == 3) return "BOOKED";
            if (dateType == 8) return "TRANSACTION";

            if (processKey == 601 || processKey == 603)
            {
                if (dateType == 1) return "ALL TICKETS";
                if (dateType == 2) return "EXPIRATION";
            }

            return string.Empty;
        }
    }
}
