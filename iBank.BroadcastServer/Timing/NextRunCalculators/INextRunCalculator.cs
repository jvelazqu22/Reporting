using System;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public interface INextRunCalculator
    {
        DateTime CalculateNextRun();
    }
}
