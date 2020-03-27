using System;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;

namespace Domain.Interfaces.BroadcastServer
{
    public interface IRecordTimingDetails
    {
        /// <summary>
        /// prevhist
        /// </summary>
        int BroadcastScheduleData { get; set; }
        
        /// <summary>
        /// weekmonth
        /// </summary>
        int FrequencyOfRun { get; set; }
        
        /// <summary>
        /// monthrun
        /// </summary>
        int DayOfMonthToRunMonthlyReport { get; set; }
        
        /// <summary>
        /// weekrun
        /// </summary>
        int DayOfWeekToRun { get; set; }
        
        /// <summary>
        /// weekstart
        /// </summary>
        int DayOfWeekOfStartingRangeForData { get; set; }
        
        /// <summary>
        /// nextdstart
        /// </summary>
        DateTime NextReportPeriodStart { get; set; }
        
        /// <summary>
        /// nextdend
        /// </summary>
        DateTime NextReportPeriodEnd { get; set; }

        /// <summary>
        /// lastdstart
        /// </summary>
        DateTime LastReportPeriodStart { get; set; }

        /// <summary>
        /// lastdend
        /// </summary>
        DateTime LastReportPeriodEnd { get; set; }

        DateTime Today { get; set; }
        DateTime Now { get; set; }

        /// <summary>
        /// runnewdata
        /// </summary>
        bool RunNewData { get; set; }

        int ReportDays { get; set; }

        int DateOfStartingRangeForMonthlyData { get; set; }

        DateTime NextRun { get; set; }

        DateTime? LastRun { get; set; }

        /// <summary>
        /// spclstart
        /// </summary>
        DateTime? SpecialReportPeriodStart { get; set; }

        /// <summary>
        /// spclend
        /// </summary>
        DateTime? SpecialReportPeriodEnd { get; set; }

        IBroadcastScheduleConditionals Conditionals { get; set; }

        ibbatch MapToBatchRecord(ibbatch batchRecord, bool runOneTimeForSpecifiedDatePeriod);

        bcstque4 MapToQueueRecord(bcstque4 queueRecord);
    }
}