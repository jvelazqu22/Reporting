namespace Domain.Interfaces.BroadcastServer
{
    public interface IBroadcastScheduleConditionals
    {
        bool IsDailyEveryXHoursSchedule { get; }
        bool IsRunDailyPriorBusinessDay { get; }
        bool IsRunDailyNextBusinessDay { get; }

        bool IsVariableRunTime { get; }
        IRecordTimingDetails BatchTiming { get; set; }
    }
}