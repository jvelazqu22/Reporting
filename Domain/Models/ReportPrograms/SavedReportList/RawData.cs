using System;
using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.SavedReportList
{
    [Exportable]
    public class RawData : IRecKey
    {
        public int RecKey { get; set; }
        public int Processkey { get; set; } = 0;
        public string Userrptnam { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public DateTime Lastused { get; set; } = DateTime.MinValue;
    }
}
