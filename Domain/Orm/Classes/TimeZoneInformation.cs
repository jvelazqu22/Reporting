namespace Domain.Orm.Classes
{
    public class TimeZoneInformation
    {
        public int RecordNo { get; set; } = 0;
        public string TimeZoneCode { get; set; } = "EST";
        public string DstAbbrev { get; set; } = "EDT";
        public string TimeZoneName { get; set; } = "Eastern Standard Time";
        public string Region { get; set; } = "North America";
        public int GmtDiff { get; set; } = -5;
    }
}
