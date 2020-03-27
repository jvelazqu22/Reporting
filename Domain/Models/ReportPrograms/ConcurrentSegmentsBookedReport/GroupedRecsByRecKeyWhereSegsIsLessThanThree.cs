using System;

namespace Domain.Models.ReportPrograms.ConcurrentSegmentsBookedReport
{
    public class GroupedRecsByRecKeyWhereSegsIsLessThanThree
    {
        public int RecKey { get; set; }
        public DateTime? FirstDate { get; set; }
        public DateTime? LastDate { get; set; }
        public int Segs { get; set; }
    }
}
