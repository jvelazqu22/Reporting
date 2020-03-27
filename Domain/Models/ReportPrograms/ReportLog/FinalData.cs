using System;

namespace Domain.Models.ReportPrograms.ReportLog
{
    public class FinalData
    {
        public string UserName { get; set; } = string.Empty;
        public DateTime RptDate { get; set; } = DateTime.MinValue;
        public string Caption { get; set; } = string.Empty;
        public string DateRange { get; set; } = string.Empty;
        public string Accts { get; set; } = string.Empty;
        public int TimesRun { get; set; } = 0;
    }
}
