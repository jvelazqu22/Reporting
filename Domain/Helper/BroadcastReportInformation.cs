using System;

namespace Domain.Helper
{
    public class BroadcastReportInformation
    {
        public BroadcastReportInformation()
        {
            UserReportName = string.Empty;
            CrystalReportType = string.Empty;
            AccountList = string.Empty;
            Usage = string.Empty;

            ReportStart = DateTime.Now;
            ReportEnd = DateTime.Now;
            IsOfflineReport = false;
            
        }
        public int BatchNumber { get; set; }
        public int BatchProgramNumber { get; set; }
        public int SavedReportNumber { get; set; }
        public int UdrKey { get; set; }
        public int ProcessKey { get; set; }
        public string Usage { get; set; }
        public int DateType { get; set; }
        public string UserReportName { get; set; }
        public string CrystalReportType { get; set; }
        public string AccountList { get; set; }
        public int PrevHist { get; set; }

        public DateTime ReportStart { get; set; }
        public DateTime ReportEnd { get; set; }
        public string LanguageCode { get; set; }
        public bool IsOfflineReport { get; set; }
        public bool IsDotNetEnabled { get; set; }
        
    }
}
