using System;

using Moq;

using iBank.Entities.MasterEntities;

using iBankDomain.RepositoryInterfaces;

namespace iBank.UnitTesting.iBank.BroadcastServer.Timing
{
    public class RecordTimingContext
    {
        public int PrevHist { get; set; }
        public int WeekMonth { get; set; }
        public int WeekRun { get; set; }      
        public int WeekStart { get; set; }
        public int MonthRun { get; set; }
        public int MonthStart { get; set; }
        public DateTime LastReportPeriodStart { get; set; }
        public DateTime LastReportPeriodEnd { get; set; }
        public DateTime ReportPeriodStart { get; set; }
        public DateTime ReportPeriodEnd { get; set; }
        public DateTime NextRun { get; set; }
        public DateTime LastRun { get; set; }
        public DateTime ActualRunTime { get; set; }

        public bool BroadcastIsOkay { get; set; }

        public int ReportNumberOfDays { get; set; }

        public DateTime Today
        {
            get
            {
                return ActualRunTime.Date;
            }
        }

        public bool IsRunSpecial { get; set; }

        public Mock<IQuery<mstragcy>> GetAgencyQuery { get; set; }

        public Mock<IQuery<MstrCorpAccts>> GetCorpAcctQuery { get; set; }
        public bool RunNewData { get; set; }

        public RecordTimingContext()
        {
            GetAgencyQuery = new Mock<IQuery<mstragcy>>();
            GetCorpAcctQuery = new Mock<IQuery<MstrCorpAccts>>();
            BroadcastIsOkay = true;
            RunNewData = false;
            IsRunSpecial = false;
        }

        
    }
}
