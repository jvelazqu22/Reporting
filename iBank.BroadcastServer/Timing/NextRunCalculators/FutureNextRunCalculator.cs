using System;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class FutureNextRunCalculator : INextRunCalculator
    {
        public DateTime CalculateNextRun()
        {
            return new DateTime(2020, 12, 31);
        }
    }
}