using CODE.Framework.Core.Utilities;
using Domain.Helper;
using Domain.Interfaces.BroadcastServer;
using iBank.Server.Utilities;
using System;

using Domain.Constants;
using Domain.Exceptions;

namespace iBank.BroadcastServer.Timing
{
    public class PreRunTimingCalculator
    {
        public void SetInitialRecordNextReportPeriodStartAndEnd(IRecordTimingDetails recordTimingDetails)
        {
            if (recordTimingDetails.RunNewData && recordTimingDetails.BroadcastScheduleData == BroadcastSchedule.RESERVATION)
            {
                var beginningOfTime = new DateTime(1900, 01, 01);
                if (recordTimingDetails.LastRun.HasValue && recordTimingDetails.LastRun.Value != beginningOfTime)
                {
                    recordTimingDetails.NextReportPeriodStart = recordTimingDetails.LastRun.Value;
                }

                recordTimingDetails.NextReportPeriodEnd = recordTimingDetails.Now.Truncate(TimeSpan.FromSeconds(1));
            }
        }

        public void SetInitialRecordStartAndEndDatesToDateTimeSafe(IRecordTimingDetails recordTimingDetails)
        {
            var isVariableRunTime = BroadcastFrequencyOfRun.IsDailyEveryXHours(recordTimingDetails.FrequencyOfRun) || BroadcastFrequencyOfRun.IsDailyEveryXMinutes(recordTimingDetails.FrequencyOfRun);
            var isReservation = recordTimingDetails.BroadcastScheduleData == BroadcastSchedule.RESERVATION;
            
            if(isReservation && isVariableRunTime)
            {
                // Safely converts a value into a DateTime or returns DateTime.MinValue if the value is invalid.
                recordTimingDetails.NextReportPeriodStart = recordTimingDetails.NextReportPeriodStart.ToDateTimeSafe();
                recordTimingDetails.NextReportPeriodEnd = recordTimingDetails.NextReportPeriodEnd.ToDateTimeSafe();

                recordTimingDetails.LastReportPeriodStart = recordTimingDetails.LastReportPeriodStart.ToDateTimeSafe();
                recordTimingDetails.LastReportPeriodEnd = recordTimingDetails.LastReportPeriodEnd.ToDateTimeSafe();
            }
            else
            {
                recordTimingDetails.NextReportPeriodStart = recordTimingDetails.NextReportPeriodStart.ToDateTimeSafe().Date;
                recordTimingDetails.NextReportPeriodEnd = recordTimingDetails.NextReportPeriodEnd.ToDateTimeSafe().Date;

                recordTimingDetails.LastReportPeriodStart = recordTimingDetails.LastReportPeriodStart.ToDateTimeSafe().Date;
                recordTimingDetails.LastReportPeriodEnd = recordTimingDetails.LastReportPeriodEnd.ToDateTimeSafe().Date;
            }
        }

        public void SetNextReportPeriodEndToEndOfDay(IRecordTimingDetails recordTimingDetails)
        {
            recordTimingDetails.NextReportPeriodEnd = recordTimingDetails.NextReportPeriodEnd.Truncate(TimeSpan.FromDays(1)).AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        public void SetRunDailyPriorBusinessDayStartAndEndRange(IRecordTimingDetails recordTimingDetails, IBatchManager batchManager)
        {
            recordTimingDetails.NextReportPeriodEnd = recordTimingDetails.Today.AddDays(-1);
            recordTimingDetails.NextReportPeriodStart = recordTimingDetails.NextReportPeriodEnd;

            switch (recordTimingDetails.Today.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    batchManager.IncrementOriginalBatchNextRun(recordTimingDetails.NextRun);
                    throw new NotTimeToRunBroadcastException(BroadcastLoggingMessage.NotTimeToRunBroadcast);
                case DayOfWeek.Monday:
                    //Run FRI-SUN, covering the weekend
                    recordTimingDetails.NextReportPeriodEnd = recordTimingDetails.Today.AddDays(-1);
                    recordTimingDetails.NextReportPeriodStart = recordTimingDetails.NextReportPeriodEnd.AddDays(-2);
                    break;
            }
        }

        public void SetRunDailyNextBusinessDayStartAndEndRange(IRecordTimingDetails recordTimingDetails, IBatchManager batchManager)
        {
            recordTimingDetails.NextReportPeriodEnd = recordTimingDetails.Today.AddDays(1);
            recordTimingDetails.NextReportPeriodStart = recordTimingDetails.NextReportPeriodEnd;

            if (recordTimingDetails.Today.DayOfWeek == DayOfWeek.Friday || recordTimingDetails.Today.DayOfWeek == DayOfWeek.Saturday)
            {
                batchManager.IncrementOriginalBatchNextRun(recordTimingDetails.NextRun);
                throw new NotTimeToRunBroadcastException(BroadcastLoggingMessage.NotTimeToRunBroadcast);
            }
        }

        public void SetReportRangeToSpecialDates(IRecordTimingDetails recordTimingDetails)
        {
            recordTimingDetails.NextReportPeriodStart = recordTimingDetails.SpecialReportPeriodStart.ToDateTimeSafe();
            recordTimingDetails.NextReportPeriodEnd = recordTimingDetails.SpecialReportPeriodEnd.ToDateTimeSafe();
        }
    }
}
