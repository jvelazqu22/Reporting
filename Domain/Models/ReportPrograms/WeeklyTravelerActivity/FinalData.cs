using System;

using Domain.Helper;

namespace Domain.Models.ReportPrograms.WeeklyTravelerActivity
{
    [Exportable]
    public class FinalData
    {
        public string BreaksFld { get; set; } = string.Empty;
        public string Day1Locn { get; set; } = string.Empty;
        public string Day2Locn { get; set; } = string.Empty;
        public string Day3Locn { get; set; } = string.Empty;
        public string Day4Locn { get; set; } = string.Empty;
        public string Day5Locn { get; set; } = string.Empty;
        public string Day6Locn { get; set; } = string.Empty;
        public string Day7Locn { get; set; } = string.Empty;
        public DateTime InvDate { get; set; } = DateTime.MaxValue;
        public string Loc1Flag { get; set; } = string.Empty;
        public string Loc2Flag { get; set; } = string.Empty;
        public string Loc3Flag { get; set; } = string.Empty;
        public string Loc4Flag { get; set; } = string.Empty;
        public string Loc5Flag { get; set; } = string.Empty;
        public string Loc6Flag { get; set; } = string.Empty;
        public string Loc7Flag { get; set; } = string.Empty;
        public string PassFrst { get; set; } = string.Empty;
        public string PassLast { get; set; } = string.Empty;
        public int Reckey { get; set; } = 0;
        public string RecLoc { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string UdidLabel { get; set; } = string.Empty;
        public int UdidNo { get; set; } = 0;
        public string UdidText { get; set; } = string.Empty;
    }

}
