namespace Domain.Models.ReportPrograms.AdvanceBookAir
{
    public class TimeBucket
    {
        public TimeBucket()
        {
            StartDay = 0;
            EndDay = 0;
        }
        public int StartDay { get; set; }
        public int EndDay { get; set; }
    }
}
