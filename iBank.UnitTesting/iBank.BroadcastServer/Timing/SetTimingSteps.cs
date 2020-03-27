using Domain.Helper;
using Domain.Interfaces.BroadcastServer;
using iBank.BroadcastServer.Timing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Domain;
using Domain.Constants;

using TechTalk.SpecFlow;

namespace iBank.UnitTesting.iBank.BroadcastServer.Timing
{
    [Binding]
    public sealed class SetTimingSteps
    {
        private RecordTimingContext _context;

        private IRecordTimingDetails _results;

        public SetTimingSteps(RecordTimingContext context)
        {
            _context = new RecordTimingContext();
        }

        #region Region - Setup
        
        //prevhist
        [Given(@"I have a broadcast scheduled for '(.*)'")]
        public void GivenIHaveABroadcastScheduledFor(string scheduleType)
        {
            if (scheduleType.Equals("Back Office")) _context.PrevHist = 2;
            else if (scheduleType.Equals("YTD Back Office, run monthly")) _context.PrevHist = BroadcastSchedule.YTD_BACK_OFFICE_RUN_MONTHLY;
            else if (scheduleType.Equals("YTD Reservation, run monthly")) _context.PrevHist = BroadcastSchedule.YTD_RESERVATION_RUN_MONTHLY;
            else if (scheduleType.Equals("YTD Back Office, run weekly")) _context.PrevHist = BroadcastSchedule.YTD_BACK_OFFICE_RUN_WEEKLY;
            else if (scheduleType.Equals("YTD Reservation, run weekly")) _context.PrevHist = BroadcastSchedule.YTD_RESERVATION_RUN_WEEKLY;
            else if (scheduleType.Equals("MTD Back Office, run weekly")) _context.PrevHist = BroadcastSchedule.MTD_BACK_OFFICE_RUN_WEEKLY;
            else if (scheduleType.Equals("MTD Reservation, run weekly")) _context.PrevHist = BroadcastSchedule.MTD_RESERVATION_RUN_WEEKLY;
            else if (scheduleType.Equals("MTD Back Office, run daily")) _context.PrevHist = BroadcastSchedule.MTD_BACK_OFFICE_RUN_DAILY;
            else if (scheduleType.Equals("MTD Reservation, run daily")) _context.PrevHist = BroadcastSchedule.MTD_RESERVATION_RUN_DAILY;
            else if (scheduleType.Equals("Reservation Mon-Wed-Fri")) _context.PrevHist = BroadcastSchedule.RESERVATION_MON_WED_FRI;
            else if (scheduleType.Equals("Reservation Sun-Tue-Thu")) _context.PrevHist = BroadcastSchedule.RESERVATION_SUN_TUE_THURS;
            else if (scheduleType.Equals("Reservation Tue-Thu-Sat")) _context.PrevHist = BroadcastSchedule.RESERVATION_TUES_THURS_SAT;
            else if (scheduleType.Equals("Reservation Weekly")) _context.PrevHist = BroadcastSchedule.RESERVATION_WEEKLY;
            else if (scheduleType.Equals("Reservation Daily")) _context.PrevHist = BroadcastSchedule.RESERVATION_DAILY;
            else if (scheduleType.Equals("Reservation")) _context.PrevHist = BroadcastSchedule.RESERVATION;
            else if (scheduleType.Equals("Reservation Previous Day")) _context.PrevHist = BroadcastSchedule.RESERVATION_PREVIOUS_DAY;
            else if (scheduleType.Equals("Reservation Previous Week")) _context.PrevHist = BroadcastSchedule.RESERVATION_PREVIOUS_WEEK;
            else if (scheduleType.Equals("Reservation Previous Month")) _context.PrevHist = BroadcastSchedule.RESERVATION_PREVIOUS_MONTH;
            else if (scheduleType.Equals("Back Office Next Week")) _context.PrevHist = BroadcastSchedule.BACK_OFFICE_NEXT_WEEK;
            else if (scheduleType.Equals("Back Office Previous Day")) _context.PrevHist = BroadcastSchedule.BACK_OFFICE_PREVIOUS_DAY;
            else throw new Exception("Broadcast Schedule Unhandled!");
        }

        //weekmonth
        [Given(@"the broadcast frequency is '(.*)'")]
        public void GivenTheBroadcastFrequencyIs(string frequency)
        {
            if (frequency.Equals("Daily")) _context.WeekMonth = 3;
            else if (frequency.Equals("Daily - Prior Business Day")) _context.WeekMonth = BroadcastFrequencyOfRun.DAILY_PRIOR_BUSINESS_DAY;
            else if (frequency.Equals("Daily - Next Business Day")) _context.WeekMonth = BroadcastFrequencyOfRun.DAILY_NEXT_BUSINESS_DAY;
            else if (frequency.Equals("Current Day")) _context.WeekMonth = BroadcastFrequencyOfRun.CURRENT_DAY;
            else if (frequency.Equals("Weekly")) _context.WeekMonth = BroadcastFrequencyOfRun.WEEKLY;
            else if (frequency.Equals("Monthly")) _context.WeekMonth = BroadcastFrequencyOfRun.MONTHLY;
            else if (frequency.Equals("Quarterly")) _context.WeekMonth = BroadcastFrequencyOfRun.QUARTERLY;
            else if (frequency.Equals("SemiAnnual")) _context.WeekMonth = BroadcastFrequencyOfRun.SEMI_ANNUAL;
            else if (frequency.Equals("Annual")) _context.WeekMonth = BroadcastFrequencyOfRun.ANNUAL;
            else if (frequency.Equals("Bi-Weekly")) _context.WeekMonth = BroadcastFrequencyOfRun.BI_WEEKLY;
            else if (frequency.Equals("Bi-Monthly")) _context.WeekMonth = BroadcastFrequencyOfRun.BI_MONTHLY;
            else if (frequency.Equals("Daily Every X Hours")) ; //this is handled in method GivenTheHourlyIntervalToRunIs
            else if (frequency.Equals("Daily Every X Minutes")) ; //this is handled in method GivenTheMinuteIntervalToRunIs
            else throw new Exception("Broadcast Frequency Unhandled!");
        }

        [Given(@"the day of the month the report runs on is '(.*)'")]

        //monthrun
        public void GivenTheDayOfTheMonthTheReportRunsOnIs(string dayOfMonth)
        {
            _context.MonthRun = int.Parse(dayOfMonth);
        }

        //monthstart
        [Given(@"the month starts on day '(.*)'")]
        public void GivenTheMonthStartsOnDay(string monthStartDay)
        {
            _context.MonthStart = int.Parse(monthStartDay);
        }

        //weekrun
        [Given(@"the report run day is '(.*)'")]
        public void GivenTheReportRunDayIs(string runDay)
        {
            switch (runDay)
            {
                case "Sunday":
                    _context.WeekRun = DayOfWeekToRun.SUNDAY;
                    break;
                case "Monday":
                    _context.WeekRun = DayOfWeekToRun.MONDAY;
                    break;
                case "Tuesday":
                    _context.WeekRun = DayOfWeekToRun.TUESDAY;
                    break;
                case "Wednesday":
                    _context.WeekRun = DayOfWeekToRun.WEDNESDAY;
                    break;
                case "Thursday":
                    _context.WeekRun = DayOfWeekToRun.THURSDAY;
                    break;
                case "Friday":
                    _context.WeekRun = DayOfWeekToRun.FRIDAY;
                    break;
                case "Saturday":
                    _context.WeekRun = DayOfWeekToRun.SATURDAY;
                    break;
                default:
                    throw new Exception("Day of Week Unhandled!");
            }
        }

        //weekstart
        [Given(@"the week starts on '(.*)'")]
        public void GivenTheWeekStartsOn(string weekStart)
        {
            switch (weekStart)
            {
                case "Monday":
                    _context.WeekStart = DayOfWeekOfStartingRangeForData.MONDAY;
                    break;
                case "Tuesday":
                    _context.WeekStart = DayOfWeekOfStartingRangeForData.GetTuesday(_context.PrevHist);
                    break;
                case "Wednesday":
                    _context.WeekStart = DayOfWeekOfStartingRangeForData.GetWednesday(_context.PrevHist);
                    break;
                case "Thursday":
                    _context.WeekStart = DayOfWeekOfStartingRangeForData.GetThursday(_context.PrevHist);
                    break;
                case "Friday":
                    _context.WeekStart = DayOfWeekOfStartingRangeForData.GetFriday(_context.PrevHist);
                    break;
                case "Saturday":
                    _context.WeekStart = DayOfWeekOfStartingRangeForData.GetSaturday(_context.PrevHist);
                    break;
                case "Sunday":
                    _context.WeekStart = DayOfWeekOfStartingRangeForData.GetSunday(_context.PrevHist);
                    break;
                default:
                    throw new Exception("Day of Week of Starting Range For Data Unhandled!");
            }
        }

        //reportdays
        [Given(@"the number of report days is '(.*)'")]
        public void GivenTheNumberOfReportDaysIs(string numberOfDays)
        {
            _context.ReportNumberOfDays = int.Parse(numberOfDays);
        }

        [Given(@"the first month of the year is '(.*)'")]
        public void GivenTheFirstMonthOfTheYearIs(string firstMonthOfYear)
        {
            switch (firstMonthOfYear)
            {
                case "January":
                    _context.WeekStart = MonthOfTheYear.JANUARY;
                    break;
                case "February":
                    _context.WeekStart = MonthOfTheYear.FEBRUARY;
                    break;
                case "March":
                    _context.WeekStart = MonthOfTheYear.MARCH;
                    break;
                case "April":
                    _context.WeekStart = MonthOfTheYear.APRIL;
                    break;
                case "May":
                    _context.WeekStart = MonthOfTheYear.MAY;
                    break;
                case "June":
                    _context.WeekStart = MonthOfTheYear.JUNE;
                    break;
                case "July":
                    _context.WeekStart = MonthOfTheYear.JULY;
                    break;
                case "August":
                    _context.WeekStart = MonthOfTheYear.AUGUST;
                    break;
                case "September":
                    _context.WeekStart = MonthOfTheYear.SEPTEMBER;
                    break;
                case "October":
                    _context.WeekStart = MonthOfTheYear.OCTOBER;
                    break;
                case "November":
                    _context.WeekStart = MonthOfTheYear.NOVEMBER;
                    break;
                case "December":
                    _context.WeekStart = MonthOfTheYear.DECEMBER;
                    break;
                default:
                    throw new Exception("Starting Month of Year Unhandled!");
            }
        }

        //startingReportPeriodStart
        [Given(@"the current report period start is '(.*)'")]
        public void GivenTheCurrentReportPeriodStartIs(string reportPeriodStart)
        {
            _context.ReportPeriodStart = Convert.ToDateTime(reportPeriodStart);
        }

        //startingReportPeriodEnd
        [Given(@"the current report period end is '(.*)'")]
        public void GivenTheCurrentReportPeriodEndIs(string reportPeriodEnd)
        {
            _context.ReportPeriodEnd = Convert.ToDateTime(reportPeriodEnd);
        }

        //last report period start
        [Given(@"the current last report period start is '(.*)'")]
        public void GivenTheCurrentLastReportPeriodStartIs(string reportPeriodStart)
        {
            _context.LastReportPeriodStart = Convert.ToDateTime(reportPeriodStart);
        }

        //last report period end
        [Given(@"the current last report period end is '(.*)'")]
        public void GivenTheCurrentLastReportPeriodEndIs(string reportPeriodEnd)
        {
            _context.LastReportPeriodEnd = Convert.ToDateTime(reportPeriodEnd);
        }

        //startingNextRun
        [Given(@"the current next run is '(.*)'")]
        public void GivenTheCurrentNextRunIs(string nextRun)
        {
            _context.NextRun = DateTime.Parse(nextRun);
        }

        //startingLastRun
        [Given(@"the broadcast last ran on '(.*)'")]
        public void GivenTheBroadcastLastRanOn(string lastRun)
        {
            _context.LastRun = DateTime.Parse(lastRun);
        }

        //actualRunTime
        [Given(@"the actual time the broadcast runs is '(.*)'")]
        public void GivenTheActualTimeTheBroadcastRunsIs(string runTime)
        {
            _context.ActualRunTime = DateTime.Parse(runTime);
        }

        //weekmonth for variable hours
        [Given(@"the hourly interval to run is '(.*)'")]
        public void GivenTheHourlyIntervalToRunIs(string hourlyInterval)
        {
            _context.WeekMonth = int.Parse(hourlyInterval) * -1;
        }

        //weekmonth for variable minutes 
        [Given(@"the minute interval to run is '(.*)'")]
        public void GivenTheMinuteIntervalToRunIs(string minuteInterval)
        {
            _context.WeekMonth = int.Parse(minuteInterval) * -1;
        }
        
        [Given(@"the broadcast '(.*)' set to run for data updated since the last time the broadcast ran")]
        public void GivenTheBroadcastSetToRunForDataUpdatedSinceTheLastTimeTheBroadcastRan(string isItSetToRunForNewData)
        {
            _context.RunNewData = isItSetToRunForNewData == "is";
        }

        [Given(@"the broadcast '(.*)' in error")]
        public void GivenTheBroadcastInError(string isNotInError)
        {
            _context.BroadcastIsOkay = isNotInError == "is not";
        }

        [Given(@"the broadcast '(.*)' run special")]
        public void GivenTheBroacastRunSpecial(string isRunSpecial)
        {
            _context.IsRunSpecial = isRunSpecial == "is";
        }

        #endregion

        //perform calculation
        [When(@"I calculate the new timing")]
        public void WhenICalculateTheNewTiming()
        {
            var postRunCalculator = new PostRunTimingCalculator();
            _results = new RecordTimingDetails()
                           {
                               BroadcastScheduleData = _context.PrevHist,
                               FrequencyOfRun = _context.WeekMonth,
                               DayOfMonthToRunMonthlyReport = _context.MonthRun,
                               DayOfWeekToRun = _context.WeekRun,
                               DayOfWeekOfStartingRangeForData = _context.WeekStart,
                               DateOfStartingRangeForMonthlyData = _context.MonthStart,
                               NextReportPeriodStart = _context.ReportPeriodStart,
                               NextReportPeriodEnd = _context.ReportPeriodEnd,
                               NextRun = _context.NextRun,
                               Now = _context.ActualRunTime,
                               Today = _context.ActualRunTime.Date,
                               RunNewData = _context.RunNewData,
                               LastReportPeriodStart = _context.LastReportPeriodStart,
                               LastReportPeriodEnd = _context.LastReportPeriodEnd,
                               ReportDays = _context.ReportNumberOfDays
                           };

            Features.BroadcastNextRunFlag = Features.AlwaysOnFlag;
            _results.Conditionals = new BroadcastScheduleConditionals(_results);
            
            postRunCalculator.SetTiming(_results, _context.IsRunSpecial, _context.BroadcastIsOkay,
                _context.GetAgencyQuery.Object, _context.GetCorpAcctQuery.Object, _context.LastReportPeriodStart);
        }

        #region Region - Results

        //last report period start
        [Then(@"the last report period start is '(.*)'")]
        public void ThenTheLastReportPeriodStartIs(string lastReportPeriodStart)
        {
            var expected = DateTime.Parse(lastReportPeriodStart);
            Assert.AreEqual(expected, _results.LastReportPeriodStart);
        }

        //last report period end
        [Then(@"the last report period end is '(.*)'")]
        public void ThenTheLastReportPeriodEndIs(string lastReportPeriodEnd)
        {
            var expected = DateTime.Parse(lastReportPeriodEnd);
            Assert.AreEqual(expected, _results.LastReportPeriodEnd);
        }

        //next report period start
        [Then(@"the next report period start is '(.*)'")]
        public void ThenTheNextReportPeriodStartIs(string nextReportPeriodStart)
        {
            var expected = DateTime.Parse(nextReportPeriodStart);
            Assert.AreEqual(expected, _results.NextReportPeriodStart);
        }

        //next report period end
        [Then(@"the next report period end is '(.*)'")]
        public void ThenTheNextReportPeriodEndIs(string nextReportPeriodEnd)
        {
            var expected = DateTime.Parse(nextReportPeriodEnd);
            Assert.AreEqual(expected, _results.NextReportPeriodEnd);
        }

        //next run
        [Then(@"the next run is '(.*)'")]
        public void ThenTheNextRunIs(string nextRun)
        {
            var expected = DateTime.Parse(nextRun);
            Assert.AreEqual(expected, _results.NextRun);
        }

        #endregion
    }
}
