using System;

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.WeeklyTravelerActivity
{
    public class UdidData : IRecKey
    {
        public int RecKey { get; set; } = 0;
        public int TripCount { get; set; } = 0;
        public Int16 UdidNo { get; set; } = 0;
        public string UdidText { get; set; } = string.Empty;
        public string UdidLabel { get; set; } = string.Empty;
    }
}
