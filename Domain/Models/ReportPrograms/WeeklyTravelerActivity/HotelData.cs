using System;

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.WeeklyTravelerActivity
{
    public class HotelData : IRecKey
    {
        public string Acct { get; set; } = string.Empty;
        public DateTime ArrDate { get; set; } = DateTime.Today;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public DateTime? DateIn { get; set; } = DateTime.Today;
        public DateTime? DateOut { get; set; } = DateTime.Today;
        public DateTime? DepDate { get; set; } = DateTime.Today;
        public string HotCity { get; set; } = string.Empty;
        public string HotState { get; set; } = string.Empty;
        public DateTime? InvDate { get; set; } = DateTime.Today;
        public string Invoice { get; set; } = string.Empty;
        public string Metro { get; set; } = string.Empty;
        public string PassFrst { get; set; } = string.Empty;
        public string PassLast { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public string RecLoc { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
    }
}
