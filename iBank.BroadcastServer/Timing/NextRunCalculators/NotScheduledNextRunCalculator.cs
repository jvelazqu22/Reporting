using System;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class NotScheduledNextRunCalculator : INextRunCalculator
    {
        public DateTime CalculateNextRun()
        {
            return new DateTime(1900, 1, 1);
        }
    }
}