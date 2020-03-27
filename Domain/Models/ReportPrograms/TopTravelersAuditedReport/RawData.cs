using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.TopTravelersAuditedReport
{
    public class RawData : IRecKey
    {
        public decimal AirChg { get; set; } = 0m;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public DateTime? Statustime { get; set; } = DateTime.MinValue;
        public string OutPolCods { get; set; } = string.Empty;
        public int TravAuthNo { get; set; } = 0;
        public string AuthStatus { get; set; } = string.Empty;
        public int SGroupNbr { get; set; } = 0;
        public decimal BookVolume { get; set; } = 0;
    }
}
