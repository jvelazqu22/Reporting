namespace Domain.Orm.Classes
{
    public class AgencyInformation
    {
        public string Agency { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public bool Active { get; set; } = false;
        public bool BcActive { get; set; } = false;
        public int TimeZoneOffset { get; set; } = 0;
    }
}
