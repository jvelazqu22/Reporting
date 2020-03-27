using System;
using Domain.Constants;
using Domain.Interfaces.BroadcastServer;
using iBank.Entities.MasterEntities;
using iBankDomain.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class NextRunCalculatorFactory : IFactory<INextRunCalculator>
    {

        private readonly IRecordTimingDetails _timing;
        
        private readonly IQuery<mstragcy> _getAgencyRecordByAgencyNameQuery;

        private readonly IQuery<MstrCorpAccts> _getCorpAcctByAgencyQuery;

        private readonly DateTime _originalLastReportPeriodStart;

        public NextRunCalculatorFactory(IRecordTimingDetails timing, IQuery<mstragcy> getAgencyRecordByAgencyNameQuery,
            IQuery<MstrCorpAccts> getCorpAcctByAgencyQuery, DateTime originalLastReportPeriodStart)
        {
            _timing = timing;
            _getAgencyRecordByAgencyNameQuery = getAgencyRecordByAgencyNameQuery;
            _getCorpAcctByAgencyQuery = getCorpAcctByAgencyQuery;
            _originalLastReportPeriodStart = originalLastReportPeriodStart;
        }

        public INextRunCalculator Build()
        {
            if (_timing.BroadcastScheduleData == BroadcastSchedule.RESERVATION
                && (BroadcastFrequencyOfRun.IsDailyEveryXHours(_timing.FrequencyOfRun) || BroadcastFrequencyOfRun.IsDailyEveryXMinutes(_timing.FrequencyOfRun)))
            {
                return new VariableRunTimeNextRunCalculator(_timing.NextRun, _timing.FrequencyOfRun, _timing.Now, _getAgencyRecordByAgencyNameQuery, _getCorpAcctByAgencyQuery);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.YTD_BACK_OFFICE_RUN_MONTHLY
                     || _timing.BroadcastScheduleData == BroadcastSchedule.YTD_RESERVATION_RUN_MONTHLY)
            {
                return new YearToDateMonthlyNextRunCalculator(_timing.NextReportPeriodEnd, _timing.DayOfMonthToRunMonthlyReport);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.MTD_BACK_OFFICE_RUN_DAILY
                     || _timing.BroadcastScheduleData == BroadcastSchedule.MTD_RESERVATION_RUN_DAILY)
            {
                return new MonthToDateDailyNextRunCalculator(_timing.NextReportPeriodEnd);
            }
            else if (_timing.FrequencyOfRun == BroadcastFrequencyOfRun.CURRENT_DAY)
            {
                return new CurrentDayNextRunCalculator(_timing.NextReportPeriodEnd);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.RESERVATION_MON_WED_FRI
                     || _timing.BroadcastScheduleData == BroadcastSchedule.RESERVATION_SUN_TUE_THURS
                     || _timing.BroadcastScheduleData == BroadcastSchedule.RESERVATION_TUES_THURS_SAT)
            {
                return new MultipleDayOfWeekNextRunCalculator(_timing.NextReportPeriodStart);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.RESERVATION_WEEKLY)
            {
                return new ReservationWeeklyNextRunCalculator(_timing.DayOfWeekToRun, _timing.Today);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.RESERVATION_DAILY)
            {
                return new ReservationDailyNextRunCalculator(_timing.NextReportPeriodStart, _timing.RunNewData, _timing.Today);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.BACK_OFFICE && _timing.FrequencyOfRun == BroadcastFrequencyOfRun.MONTHLY)
            {
                return new MonthlyCalendarRangeNextRunCalculator(_timing.NextReportPeriodStart, _timing.DayOfMonthToRunMonthlyReport, CalendarRange.Monthly);
            }
            else if ((_timing.BroadcastScheduleData == BroadcastSchedule.BACK_OFFICE || _timing.BroadcastScheduleData == BroadcastSchedule.RESERVATION)
                && _timing.FrequencyOfRun == BroadcastFrequencyOfRun.BI_MONTHLY)
            {
                return new MonthlyCalendarRangeNextRunCalculator(_timing.NextRun, _timing.DayOfMonthToRunMonthlyReport, CalendarRange.BiMonthly);
            }
            else if (_timing.FrequencyOfRun == BroadcastFrequencyOfRun.QUARTERLY)
            {
                return new MonthlyCalendarRangeNextRunCalculator(_timing.NextReportPeriodStart, _timing.DayOfMonthToRunMonthlyReport, CalendarRange.Quarter);
            }
            else if (_timing.FrequencyOfRun == BroadcastFrequencyOfRun.SEMI_ANNUAL)
            {
                return new MonthlyCalendarRangeNextRunCalculator(_timing.NextReportPeriodStart, _timing.DayOfMonthToRunMonthlyReport, CalendarRange.SemiAnnual);
            }
            else if (_timing.FrequencyOfRun == BroadcastFrequencyOfRun.ANNUAL)
            {
                return new MonthlyCalendarRangeNextRunCalculator(_timing.NextReportPeriodStart, _timing.DayOfMonthToRunMonthlyReport, CalendarRange.Annual);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.RESERVATION && _timing.FrequencyOfRun == BroadcastFrequencyOfRun.MONTHLY)
            {
                return new MonthlyCalendarRangeNextRunCalculator(_originalLastReportPeriodStart, _timing.DayOfMonthToRunMonthlyReport, CalendarRange.Monthly);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.RESERVATION_PREVIOUS_MONTH && _timing.FrequencyOfRun == BroadcastFrequencyOfRun.MONTHLY)
            {
                return new MonthlyCalendarRangeNextRunCalculator(_timing.NextReportPeriodStart, _timing.DayOfMonthToRunMonthlyReport, CalendarRange.Monthly);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.YTD_BACK_OFFICE_RUN_WEEKLY
                     || _timing.BroadcastScheduleData == BroadcastSchedule.MTD_BACK_OFFICE_RUN_WEEKLY
                     || _timing.BroadcastScheduleData == BroadcastSchedule.YTD_RESERVATION_RUN_WEEKLY
                     || _timing.BroadcastScheduleData == BroadcastSchedule.MTD_RESERVATION_RUN_WEEKLY
                     || (_timing.BroadcastScheduleData == BroadcastSchedule.BACK_OFFICE
                         && _timing.FrequencyOfRun == BroadcastFrequencyOfRun.WEEKLY
                         && _timing.DayOfWeekOfStartingRangeForData == DayOfWeekOfStartingRangeForData.MONDAY))
            {
                return new YTDOrMTDOrBackOfficeMondayWeeklyNextRunCalculator(_timing.NextReportPeriodEnd, _timing.DayOfWeekToRun, _timing.Today);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.BACK_OFFICE && _timing.FrequencyOfRun == BroadcastFrequencyOfRun.WEEKLY
                     && _timing.DayOfWeekOfStartingRangeForData == DayOfWeekOfStartingRangeForData.GetTuesday(_timing.BroadcastScheduleData))
            {
                //the FP still handles Weekly MTD Back Office and Weekly YTD Back Office in this conditional, but there is no point since it is handled above
                return new BackOfficeTuesdayWeeklyNextRunCalculator(_timing.NextReportPeriodEnd, _timing.DayOfWeekToRun, _timing.Today);
            }
            else if ((_timing.BroadcastScheduleData == BroadcastSchedule.RESERVATION || _timing.BroadcastScheduleData == BroadcastSchedule.BACK_OFFICE)
                     && _timing.FrequencyOfRun == BroadcastFrequencyOfRun.WEEKLY)
            {
                return new BackOfficeOrReservationWeeklyNextRunCalculator(_timing.NextReportPeriodStart, _timing.NextReportPeriodEnd,
                    _timing.BroadcastScheduleData, _timing.DayOfWeekToRun);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.RESERVATION_PREVIOUS_WEEK
                     && _timing.FrequencyOfRun == BroadcastFrequencyOfRun.WEEKLY)
            {
                return new ReservationPreviousWeekWeeklyNextRunCalculator(_timing.NextReportPeriodEnd, _timing.DayOfWeekToRun, _timing.RunNewData,
                    _timing.Today);
            }
            else if ((_timing.BroadcastScheduleData == BroadcastSchedule.RESERVATION_PREVIOUS_DAY
                      || _timing.BroadcastScheduleData == BroadcastSchedule.BACK_OFFICE_PREVIOUS_DAY)
                     && _timing.FrequencyOfRun == BroadcastFrequencyOfRun.DAILY)
            {
                return new PreviousDayDailyNextRunCalculator(_timing.NextReportPeriodStart, _timing.RunNewData, _timing.Today);
            }
            else if (_timing.FrequencyOfRun == BroadcastFrequencyOfRun.DAILY_PRIOR_BUSINESS_DAY
                     || _timing.FrequencyOfRun == BroadcastFrequencyOfRun.DAILY_NEXT_BUSINESS_DAY)
            {
                return new DailyBusinessDayNextRunCalculator(_timing.Today, _timing.NextRun);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.RESERVATION
                     && _timing.FrequencyOfRun == BroadcastFrequencyOfRun.DAILY)
            {
                return new ReservationDailyFrequencyNextRunCalculator(_timing.NextReportPeriodStart);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.BACK_OFFICE
                     && _timing.FrequencyOfRun == BroadcastFrequencyOfRun.DAILY)
            {
                return new BackOfficeDailyFrequencyNextRunCalculator(_timing.NextReportPeriodStart);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.BACK_OFFICE_NEXT_WEEK
                     && _timing.DayOfWeekOfStartingRangeForData == DayOfWeekOfStartingRangeForData.GetSunday(_timing.BroadcastScheduleData))
            {
                return new BackOfficeNextWeekSundayNextRunCalculator(_timing.DayOfWeekToRun, _timing.NextReportPeriodStart);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.BACK_OFFICE_NEXT_WEEK
                     && _timing.DayOfWeekOfStartingRangeForData == DayOfWeekOfStartingRangeForData.MONDAY)
            {
                return new BackOfficeNextWeekMondayNextRunCalculator(_timing.DayOfWeekToRun, _timing.NextReportPeriodStart);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.BACK_OFFICE_NEXT_WEEK
                     && _timing.DayOfWeekOfStartingRangeForData == DayOfWeekOfStartingRangeForData.GetTuesday(_timing.BroadcastScheduleData))
            {
                return new BackOfficeNextWeekTuesdayNextRunCalculator(_timing.DayOfWeekToRun, _timing.NextReportPeriodStart);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.BACK_OFFICE_NEXT_WEEK
                     && _timing.DayOfWeekOfStartingRangeForData == DayOfWeekOfStartingRangeForData.GetWednesday(_timing.BroadcastScheduleData))
            {
                return new BackOfficeNextWeekWednesdayNextRunCalculator(_timing.DayOfWeekToRun, _timing.NextReportPeriodStart);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.BACK_OFFICE_NEXT_WEEK
                     && _timing.DayOfWeekOfStartingRangeForData == DayOfWeekOfStartingRangeForData.GetThursday(_timing.BroadcastScheduleData))
            {
                return new BackOfficeNextWeekThursdayNextRunCalculator(_timing.DayOfWeekToRun, _timing.NextReportPeriodStart);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.BACK_OFFICE_NEXT_WEEK
                     && _timing.DayOfWeekOfStartingRangeForData == DayOfWeekOfStartingRangeForData.GetFriday(_timing.BroadcastScheduleData))
            {
                return new BackOfficeNextWeekFridayNextRunCalculator(_timing.DayOfWeekToRun, _timing.NextReportPeriodStart);
            }
            else if (_timing.BroadcastScheduleData == BroadcastSchedule.BACK_OFFICE_NEXT_WEEK
                     && _timing.DayOfWeekOfStartingRangeForData == DayOfWeekOfStartingRangeForData.GetSaturday(_timing.BroadcastScheduleData))
            {
                return new BackOfficeNextWeekSaturdayNextRunCalculator(_timing.DayOfWeekToRun, _timing.NextReportPeriodStart);
            }
            else if ((_timing.BroadcastScheduleData == BroadcastSchedule.RESERVATION || _timing.BroadcastScheduleData == BroadcastSchedule.BACK_OFFICE) && _timing.FrequencyOfRun == BroadcastFrequencyOfRun.BI_WEEKLY)
            {
                return new BiWeeklyNextRunCalculator(_timing);
            }
            else if (_timing.FrequencyOfRun == BroadcastFrequencyOfRun.NOT_SCHEDULED)
            {
                return new NotScheduledNextRunCalculator();
            }
            else
            {
                return new FutureNextRunCalculator();
            }
        }


    }
}