using System;
using Domain.Constants;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class MonthlyCalendarRangeNextRunCalculator : INextRunCalculator
    {
        
        private int _dayOfMonthToRunMonthlyReport;
        private DateTime _dateToBaseIncrementOff;

        private CalendarRange _range;

        public MonthlyCalendarRangeNextRunCalculator(DateTime dateToBaseIncrementOff, int dayOfMonthToRunMonthlyReport, CalendarRange range)
        {
            _dateToBaseIncrementOff = dateToBaseIncrementOff;
            _dayOfMonthToRunMonthlyReport = dayOfMonthToRunMonthlyReport;
            _range = range;
        }

        public DateTime CalculateNextRun()
        {
            var timingCalculator = new TimingCalculator();
            var monthsToAdd = 0;
            switch (_range)
            {
                case CalendarRange.Monthly:
                    monthsToAdd = 1;
                    break;
                case CalendarRange.BiMonthly:
                    monthsToAdd = 2;
                    break;
                case CalendarRange.Quarter:
                    monthsToAdd = 3;
                    break;
                case CalendarRange.SemiAnnual:
                    monthsToAdd = 6;
                    break;
                case CalendarRange.Annual:
                    monthsToAdd = 12;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(_range.ToString());
            }
            var temp = _dateToBaseIncrementOff.AddMonths(monthsToAdd);
            return timingCalculator.GetValidDate(temp.Year, temp.Month, _dayOfMonthToRunMonthlyReport);
        }
    }
}