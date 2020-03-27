using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Models.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    public interface INextReportPeriodCalculator
    {
        ReportPeriodDateRange CalculateNextReportPeriod();
    }
}
