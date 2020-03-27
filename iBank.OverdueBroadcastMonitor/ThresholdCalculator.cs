using System;

namespace iBank.OverdueBroadcastMonitor
{
    public static class ThresholdCalculator
    {
        public static DateTime CalculateThreshold(DateTime baseDate, int thresholdInMinutes)
        {
            return baseDate.AddMinutes(-thresholdInMinutes);
        }
    }
}
