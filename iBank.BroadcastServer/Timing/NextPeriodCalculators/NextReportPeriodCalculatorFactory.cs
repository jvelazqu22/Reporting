using Domain.Constants;
using Domain.Interfaces.BroadcastServer;

using iBankDomain.Interfaces;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    public class NextReportPeriodCalculatorFactory : IFactory<INextReportPeriodCalculator>
    {
        private IRecordTimingDetails Timing { get; set; }
        public NextReportPeriodCalculatorFactory(IRecordTimingDetails timing)
        {
            Timing = timing;
        }
        public INextReportPeriodCalculator Build()
        {
            //based on prevhist column
            switch (Timing.BroadcastScheduleData)
            {
                case BroadcastSchedule.YTD_BACK_OFFICE_RUN_MONTHLY:
                case BroadcastSchedule.YTD_RESERVATION_RUN_MONTHLY:

                    return new YearToDateMonthlyCalculator(Timing);

                case BroadcastSchedule.YTD_BACK_OFFICE_RUN_WEEKLY:
                case BroadcastSchedule.YTD_RESERVATION_RUN_WEEKLY:

                    return new YearToDateWeeklyCalculator(Timing);

                case BroadcastSchedule.MTD_BACK_OFFICE_RUN_WEEKLY:
                case BroadcastSchedule.MTD_RESERVATION_RUN_WEEKLY:

                    return new MonthToDateWeeklyCalculator(Timing);

                case BroadcastSchedule.MTD_BACK_OFFICE_RUN_DAILY:
                case BroadcastSchedule.MTD_RESERVATION_RUN_DAILY:

                    return new MonthToDateDailyCalculator(Timing);

                case BroadcastSchedule.RESERVATION_MON_WED_FRI:

                    return new ReservationMonWedFriCalculator(Timing);

                case BroadcastSchedule.RESERVATION_SUN_TUE_THURS:

                    return new ReservationSunTuesThursCalculator(Timing);

                case BroadcastSchedule.RESERVATION_TUES_THURS_SAT:

                    return new ReservationTuesThursSatCalculator(Timing);

                case BroadcastSchedule.RESERVATION_WEEKLY:

                    return new ReservationWeeklyCalculator(Timing);

                case BroadcastSchedule.RESERVATION_DAILY:

                    return new ReservationDailyCalculator(Timing);
            }

            //weekmonth
            switch (Timing.FrequencyOfRun)
            {
                case BroadcastFrequencyOfRun.DAILY:
                case BroadcastFrequencyOfRun.CURRENT_DAY:
                    return new RunDailyOrCurrentDayCalculator(Timing);

                case BroadcastFrequencyOfRun.DAILY_PRIOR_BUSINESS_DAY:
                    return new RunDailyPriorBusinessDayCalculator(Timing);

                case BroadcastFrequencyOfRun.DAILY_NEXT_BUSINESS_DAY:

                    return new RunDailyNextBusinessDayCalculator(Timing);

                case BroadcastFrequencyOfRun.WEEKLY:
                    return new RunWeeklyCalculator(Timing);

                case BroadcastFrequencyOfRun.QUARTERLY:
                    return new RunQuarterlyCalculator(Timing);

                case BroadcastFrequencyOfRun.SEMI_ANNUAL:
                    return new RunSemiAnnualCalculator(Timing);

                case BroadcastFrequencyOfRun.ANNUAL:
                    return new RunAnnualCalculator(Timing);

                case BroadcastFrequencyOfRun.BI_MONTHLY:
                    return new RunBiMonthlyCalculator(Timing);

                case BroadcastFrequencyOfRun.BI_WEEKLY:
                    return new RunBiWeeklyCalculator(Timing);
            }

            //is daily every x minutes
            if (Timing.BroadcastScheduleData == 1 && Timing.FrequencyOfRun < 0)
            {
                //handle the run new data option here as well
                return new VariableTimePeriodCalculator(Timing);
            }

            if (Timing.BroadcastScheduleData == 2 && Timing.FrequencyOfRun == 3)
            {
                return new BackOfficeDailyCalculator(Timing);
            }

            return new GeneralReportPeriodCalculator(Timing);


        }
    }
}
