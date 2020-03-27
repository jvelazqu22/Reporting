using System;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ReportLog
{
    public class RawData  : IRecKey
    {
        public int RecKey { get; set; }
        public int RptLogNo { get; set; } = 0;
        public int UserNumber { get; set; } = 0;
        public string UserName { get; set; } = string.Empty;
        public DateTime RptDate { get; set; } = DateTime.MinValue;
        public int ProcessKey { get; set; } = 0;
        public string RptProgram { get; set; } = string.Empty;
    }
}
