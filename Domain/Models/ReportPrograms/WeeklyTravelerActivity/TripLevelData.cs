using System;

using Domain.Helper;

namespace Domain.Models.ReportPrograms.WeeklyTravelerActivity
{
    [Exportable]
    public class TripLevelData
    {
        public string Acct { get; set; } = string.Empty;
        public DateTime? ArrDate { get; set; } = DateTime.Today;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public DateTime? DepDate { get; set; } = DateTime.Today;
        public DateTime? InvDate { get; set; } = DateTime.Today;
        public int NumLegs { get; set; } = 0;
        public string PassFrst { get; set; } = string.Empty;
        public string PassLast { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public string RecLoc { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
    }
}
