using Domain.Interfaces.BroadcastServer;
using System.Collections.Generic;

namespace iBank.BroadcastServer.Timing
{
    public class BroadcastScheduleConditionals : IBroadcastScheduleConditionals
    {
        private static readonly IList<int> _weeklyReports = new List<int> { 32, 35, 34, 37 };
        
        public bool IsDailyEveryXHoursSchedule {
            get
            {
                return BatchTiming.FrequencyOfRun <= -1 && BatchTiming.FrequencyOfRun >= -23;
            }
        }
        
        public bool IsRunDailyPriorBusinessDay
        {
            get
            {
                return BatchTiming.FrequencyOfRun == 31;
            }
        }

        public bool IsRunDailyNextBusinessDay
        {
            get
            {
                return BatchTiming.FrequencyOfRun == 32;
            }
        }

        public bool IsVariableRunTime
        {
            get
            {
                return BatchTiming.FrequencyOfRun < 0;
            }
        }

        public IRecordTimingDetails BatchTiming { get; set; }

        public BroadcastScheduleConditionals(IRecordTimingDetails batchTiming)
        {
            BatchTiming = batchTiming;
        }
    }
}
