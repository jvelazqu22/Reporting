using CODE.Framework.Core.Utilities;

using Domain.Interfaces.BroadcastServer;
using iBank.BroadcastServer.Timing.NextPeriodCalculators;
using iBank.Server.Utilities;

using System;
using Domain;
using Domain.Constants;
using Domain.Models.BroadcastServer;
using iBank.BroadcastServer.Timing.NextRunCalculators;
using iBank.Entities.MasterEntities;

using iBankDomain.RepositoryInterfaces;

namespace iBank.BroadcastServer.Timing
{
    public class PostRunTimingCalculator
    {
        public void SetTiming(IRecordTimingDetails timing, bool batchIsRunSpecial, bool batchIsOk, IQuery<mstragcy> getAgencyQuery, IQuery<MstrCorpAccts> getCorpAcctByAgencyQuery,
            DateTime originalLastStart)
        {
            UpdateLastPeriodDates(timing);

            //update the next report period
            var nextReportPeriod = GetNextReportStartAndEnd(batchIsRunSpecial, batchIsOk, timing);
            timing.NextReportPeriodStart = nextReportPeriod.ReportPeriodStart;
            timing.NextReportPeriodEnd = nextReportPeriod.ReportPeriodEnd;

            //update the next run
            timing.NextRun = GetNextRun(batchIsRunSpecial, batchIsOk, timing, getAgencyQuery, getCorpAcctByAgencyQuery, originalLastStart);

            //logic fix via FoxPro - line 2598
            if (timing.BroadcastScheduleData == BroadcastSchedule.RESERVATION_WEEKLY)
            {
                timing.NextReportPeriodStart = timing.NextRun.Date.AddDays(1);
                timing.NextReportPeriodEnd = timing.NextReportPeriodStart.AddDays(timing.ReportDays - 1);
            }

            //truncate fields to fit the backing data types
            SanitizeDates(timing);
        }

        private ReportPeriodDateRange GetNextReportStartAndEnd(bool batchIsRunSpecial, bool batchIsErrorFree, IRecordTimingDetails timing)
        {
            if (!batchIsErrorFree || batchIsRunSpecial)
            {
                return new ReportPeriodDateRange
                {
                    ReportPeriodStart = timing.NextReportPeriodStart,
                    ReportPeriodEnd = timing.NextReportPeriodEnd
                };
            }

            var nextReportPeriodFactory = new NextReportPeriodCalculatorFactory(timing);
            var calc = nextReportPeriodFactory.Build();

            return calc.CalculateNextReportPeriod();
        }

        private DateTime GetNextRun(bool batchIsRunSpecial, bool batchIsErrorFree, IRecordTimingDetails timing, IQuery<mstragcy> getAgencyRecordByAgencyNameQuery,
            IQuery<MstrCorpAccts> getCorpAcctByAgencyQuery, DateTime originalLastPeriodStart)
        {
            if (!batchIsErrorFree || batchIsRunSpecial) return timing.NextRun;
            
            var oldTime = new Time(timing.NextRun.Hour, timing.NextRun.Minute, timing.NextRun.Second);

            var nextRunFactory = new NextRunCalculatorFactory(timing, getAgencyRecordByAgencyNameQuery, getCorpAcctByAgencyQuery, originalLastPeriodStart);
            var calc = nextRunFactory.Build();

            var nextRun = calc.CalculateNextRun();
            
            if (Features.BroadcastNextRunFlag.IsEnabled())
            {
                if (IsStartOfTime(nextRun)) return nextRun;

                //for variable broadcasts we want to actually increment all the pieces of the next run [year/month/day/hour/min/sec]
                if (timing.BroadcastScheduleData == BroadcastSchedule.RESERVATION && IsVariableRunTime(timing.FrequencyOfRun)) return nextRun;

                //for broadcasts other than variable times we want to hang on to the hour/min/sec of the previous next run and only increment the day/month/year
                return new DateTime(nextRun.Year, nextRun.Month, nextRun.Day, oldTime.Hour, oldTime.Minute, oldTime.Second);
            }
            else
            {
                if (nextRun != new DateTime(1900, 1, 1))
                {
                    var isVariableTime = BroadcastFrequencyOfRun.IsDailyEveryXMinutes(timing.FrequencyOfRun) || BroadcastFrequencyOfRun.IsDailyEveryXHours(timing.FrequencyOfRun);

                    //TODO: delete IsDateType extension when removing this feature flag
                    if (!(timing.BroadcastScheduleData == BroadcastSchedule.RESERVATION && isVariableTime) || nextRun.IsDateType())
                    {
                        var year = nextRun.Year;
                        var month = nextRun.Month;
                        var day = nextRun.Day;
                        return new DateTime(year, month, day, oldTime.Hour, oldTime.Minute, oldTime.Second);
                    }
                }

                return nextRun.Truncate(TimeSpan.FromSeconds(1));
            }
        }
        
        private bool IsStartOfTime(DateTime dt) => dt == new DateTime(1900, 1, 1);

        private bool IsVariableRunTime(int frequencyOfRun) => BroadcastFrequencyOfRun.IsDailyEveryXMinutes(frequencyOfRun) || BroadcastFrequencyOfRun.IsDailyEveryXHours(frequencyOfRun);
        
        /// <summary>
        /// The ibbatch table is a SMALLDATETIME, so if we insert with the seconds > 30 it will round up, making the date wrong.
        /// So we truncate to the minute, so that our date doesn't change.
        /// </summary>
        private void SanitizeDates(IRecordTimingDetails timing)
        {
            timing.NextReportPeriodEnd = timing.NextReportPeriodEnd.Truncate(TimeSpan.FromMinutes(1));
            timing.NextReportPeriodStart = timing.NextReportPeriodStart.Truncate(TimeSpan.FromMinutes(1));
            timing.LastReportPeriodEnd = timing.LastReportPeriodEnd.Truncate(TimeSpan.FromMinutes(1));
            timing.LastReportPeriodStart = timing.LastReportPeriodStart.Truncate(TimeSpan.FromMinutes(1));
            timing.LastRun = timing.LastRun.ToDateTimeSafe().Truncate(TimeSpan.FromMinutes(1));
        }

        private void UpdateLastPeriodDates(IRecordTimingDetails timing)
        {
            timing.LastReportPeriodStart = timing.NextReportPeriodStart;
            timing.LastReportPeriodEnd = timing.NextReportPeriodEnd;
        }

        private struct Time
        {
            public int Hour { get; set; }

            public int Minute { get; set; }

            public int Second { get; set; }

            public Time(int hour, int minute, int second)
            {
                Hour = hour;
                Minute = minute;
                Second = second;
            }
        }
    }
}
