﻿using Domain.Interfaces.BroadcastServer;
using Domain.Models.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    public class BackOfficeDailyCalculator : INextReportPeriodCalculator
    {
        private IRecordTimingDetails _timing;

        public BackOfficeDailyCalculator(IRecordTimingDetails timing)
        {
            _timing = timing;
        }

        public ReportPeriodDateRange CalculateNextReportPeriod()
        {
            return new ReportPeriodDateRange
                       {
                           ReportPeriodStart = _timing.NextReportPeriodStart.AddDays(1),
                           ReportPeriodEnd = _timing.NextReportPeriodEnd.AddDays(1)
                       };
        }
    }
}