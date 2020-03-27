using System;

namespace Domain.Orm.Classes
{
    public class PendingReportInformation
    {
        public PendingReportInformation()
        {
            ReportId = string.Empty;
            ColdFusionBox = string.Empty;
            Agency = string.Empty;
            ReturnCode = 1;
            ErrorMessage = string.Empty;
            Href = string.Empty;
            IsDemoUser = false;
            ReturnText = "";
        }
        
        public DateTime DateCreated { get; set; }
        public string ReportId { get; set; }
        public string ColdFusionBox { get; set; }
        public string Agency { get; set; }
        public int UserNumber { get; set; }
        public int ServerNumber { get; set; }
        public int ReturnCode { get; set; }
        public string Href { get; set; }
        public int ReportLogKey { get; set; }
        public int RecordCount { get; set; }

        public bool IsDemoUser { get; set; }

        public string ErrorMessage { get; set; }

        public string ReturnText { get; set; }
        /// <summary>
        /// Corresponds to the ibankmasters.dbo.pending_reports.row_version field
        /// </summary>
        public byte[] RowVersion { get; set; }
        /// <summary>
        /// Corresponds to the ibankmasters.dbo.pending_reports.is_dotnet field
        /// </summary>
        public bool IsDotNet { get; set; }
    }
}
