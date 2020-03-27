using Domain.Interfaces.BroadcastServer;

using System;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;

namespace iBank.BroadcastServer.Timing
{
    public class RecordTimingDetails : IRecordTimingDetails
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
        /// ibbatch value - prevhist
        /// </summary>
        public int BroadcastScheduleData { get; set; }

        /// <summary>
        /// ibbatch value - weekmonth
        /// </summary>
        public int FrequencyOfRun { get; set; }

        /// <summary>
        /// ibbatch value - monthrun
        /// </summary>
        public int DayOfMonthToRunMonthlyReport { get; set; }

        /// <summary>
        /// ibbatch value - weekrun
        /// </summary>
        public int DayOfWeekToRun { get; set; }

        /// <summary>
        /// ibbatch value - weekstart
        /// </summary>
        public int DayOfWeekOfStartingRangeForData { get; set; }

        /// <summary>
        /// ibbatch value - monthstart
        /// </summary>
        public int DateOfStartingRangeForMonthlyData { get; set; }

        /// <summary>
        /// ibbatch value - nextdstart
        /// </summary>
        public DateTime NextReportPeriodStart { get; set; }

        /// <summary>
        /// ibbatch value - nextdend
        /// </summary>
        public DateTime NextReportPeriodEnd { get; set; }

        /// <summary>
        /// lastdstart
        /// </summary>
        public DateTime LastReportPeriodStart { get; set; }

        /// <summary>
        /// lastdend
        /// </summary>
        public DateTime LastReportPeriodEnd { get; set; }

        public DateTime Today { get; set; }
        public DateTime Now { get; set; }

        /// <summary>
        /// ibbatch value - runnewdata
        /// </summary>
        public bool RunNewData { get; set; }

        public int ReportDays { get; set; }

        public DateTime NextRun { get; set; }

        public DateTime? LastRun { get; set; }

        public DateTime? SpecialReportPeriodStart { get; set; }
        public DateTime? SpecialReportPeriodEnd { get; set; }

        public IBroadcastScheduleConditionals Conditionals { get; set; }

        public RecordTimingDetails()
        {
        }

        public RecordTimingDetails(ibbatch batchRecord)
        {
            BroadcastScheduleData = batchRecord.prevhist.Value;
            FrequencyOfRun = batchRecord.weekmonth.Value;
            DayOfMonthToRunMonthlyReport = batchRecord.monthrun.Value;
            DayOfWeekToRun = batchRecord.weekrun.Value;
            DayOfWeekOfStartingRangeForData = batchRecord.weekstart.Value;
            NextReportPeriodStart = batchRecord.nxtdstart.Value;
            NextReportPeriodEnd = batchRecord.nxtdend.Value;
            RunNewData = batchRecord.RunNewData == 1;
            LastReportPeriodEnd = batchRecord.lastdend.Value;
            LastReportPeriodStart = batchRecord.lastdstart.Value;
            ReportDays = batchRecord.reportdays ?? 0;
            DateOfStartingRangeForMonthlyData = batchRecord.monthstart.Value;
            NextRun = batchRecord.nextrun.Value;
            LastRun = batchRecord.lastrun.Value;
            SpecialReportPeriodStart = batchRecord.spclstart;
            SpecialReportPeriodEnd = batchRecord.spclend;
            Today = DateTime.Today;
            Now = DateTime.Now;

            Conditionals = new BroadcastScheduleConditionals(this);
        }

        public RecordTimingDetails(bcstque4 queueRecord)
        {
            BroadcastScheduleData = queueRecord.prevhist.Value;
            FrequencyOfRun = queueRecord.weekmonth.Value;
            DayOfMonthToRunMonthlyReport = queueRecord.monthrun.Value;
            DayOfWeekToRun = queueRecord.weekrun.Value;
            DayOfWeekOfStartingRangeForData = queueRecord.weekstart.Value;
            NextReportPeriodStart = queueRecord.nxtdstart.Value;
            NextReportPeriodEnd = queueRecord.nxtdend.Value;
            RunNewData = queueRecord.runnewdata == 1;
            LastReportPeriodEnd = queueRecord.lastdend.Value;
            LastReportPeriodStart = queueRecord.lastdstart.Value;
            ReportDays = queueRecord.reportdays ?? 0;
            DateOfStartingRangeForMonthlyData = queueRecord.monthstart.Value;
            NextRun = queueRecord.nextrun.Value;
            LastRun = queueRecord.lastrun.Value;
            SpecialReportPeriodStart = queueRecord.spclstart;
            SpecialReportPeriodEnd = queueRecord.spclend;
            Today = DateTime.Today;
            Now = DateTime.Now;

            Conditionals = new BroadcastScheduleConditionals(this);
        }

        public bcstque4 MapToQueueRecord(bcstque4 queueRecord)
        {
            queueRecord.prevhist = BroadcastScheduleData;
            queueRecord.weekmonth = FrequencyOfRun;
            queueRecord.monthrun = DayOfMonthToRunMonthlyReport;
            queueRecord.weekrun = DayOfWeekToRun;
            queueRecord.weekstart = DayOfWeekOfStartingRangeForData;
            queueRecord.nxtdstart = NextReportPeriodStart;
            queueRecord.nxtdend = NextReportPeriodEnd;
            queueRecord.runnewdata = RunNewData ? (byte)1 : (byte)0;
            queueRecord.lastdend = LastReportPeriodEnd;
            queueRecord.lastdstart = LastReportPeriodStart;
            queueRecord.reportdays = ReportDays;
            queueRecord.monthstart = DateOfStartingRangeForMonthlyData;
            queueRecord.nextrun = NextRun;
            queueRecord.lastrun = LastRun;
            queueRecord.spclstart = SpecialReportPeriodStart;
            queueRecord.spclend = SpecialReportPeriodEnd;

            return queueRecord;
        }

        public ibbatch MapToBatchRecord(ibbatch batchRecord, bool runOneTimeForSpecifiedDatePeriod)
        {
            batchRecord.prevhist = BroadcastScheduleData;
            batchRecord.weekmonth = FrequencyOfRun;
            batchRecord.monthrun = DayOfMonthToRunMonthlyReport;
            batchRecord.weekrun = DayOfWeekToRun;
            batchRecord.weekstart = DayOfWeekOfStartingRangeForData;
            batchRecord.nxtdstart = runOneTimeForSpecifiedDatePeriod ? batchRecord.nxtdstart : NextReportPeriodStart;
            batchRecord.nxtdend = runOneTimeForSpecifiedDatePeriod ? batchRecord.nxtdend : NextReportPeriodEnd;
            batchRecord.RunNewData = RunNewData ? (byte)1 : (byte)0;
            batchRecord.lastdend = runOneTimeForSpecifiedDatePeriod ? batchRecord.lastdend : LastReportPeriodEnd;
            batchRecord.lastdstart = runOneTimeForSpecifiedDatePeriod ? batchRecord.lastdstart : LastReportPeriodStart;
            batchRecord.reportdays = ReportDays;
            batchRecord.monthstart = DateOfStartingRangeForMonthlyData;
            batchRecord.nextrun = NextRun;
            batchRecord.lastrun = runOneTimeForSpecifiedDatePeriod ? batchRecord.lastrun : LastRun;
            batchRecord.spclstart = SpecialReportPeriodStart;
            batchRecord.spclend = SpecialReportPeriodEnd;

            return batchRecord;
        }
        
    }
}
