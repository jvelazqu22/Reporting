
namespace iBank.Services.Orm.Classes
{
    public class TimeZoneInformation
    {
        public TimeZoneInformation()
        {
            RecordNo = 0;
            TimeZoneCode = "EST";
            GmtDiff = -5;
            TimeZoneName = "Eastern Standard Time";
            DstAbbrev = "EDT";
            Region = "North America";
        }

        public int RecordNo { get; set; }
        public string TimeZoneCode { get; set; }
        public string DstAbbrev { get; set; }
        public string TimeZoneName { get; set; }
        public string Region { get; set; }
        public int GmtDiff { get; set; }
    }
}
