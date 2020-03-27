using System;
using Domain.Interfaces.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    internal class BiWeeklyNextRunCalculator : INextRunCalculator
    {
        private IRecordTimingDetails _timing;

        public BiWeeklyNextRunCalculator(IRecordTimingDetails timing)
        {
            _timing = timing;
        }

        public DateTime CalculateNextRun()
        {
            return _timing.NextRun.AddDays(14);
        }
    }
}