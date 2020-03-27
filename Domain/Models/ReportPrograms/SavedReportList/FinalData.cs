using System;
using Domain.Helper;

namespace Domain.Models.ReportPrograms.SavedReportList
{
    [Exportable]
    public class FinalData
    {
        public int Processkey { get; set; } = 0;
        public string Userrptnam { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public DateTime Lastused { get; set; } = DateTime.MinValue;
    }
}
