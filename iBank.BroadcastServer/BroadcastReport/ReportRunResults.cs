using System;

namespace iBank.BroadcastServer.BroadcastReport
{
    public class ReportRunResults
    {
        public bool ReportRunSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public int ReturnCode { get; set; }
        public string ReportHref { get; set; }
        public bool ReportHasNoData { get; set; }
        public string eFFECTSDeliveryReturnMessage { get; set; }
        //These properties will hold the dates needed by the email in the cases where the original date is changed by the reporting program. 
        public DateTime ReportPeriodStartDate { get; set; }
        public DateTime ReportPeriodEndDate { get; set; }
        public int FinalRecordsCount { get; set; } = 0;
        public string ReportFileName { get; set; } = string.Empty;
        public string ReportId => ReportFileName.ToUpper().Replace(".PDF", "");
    }
}
