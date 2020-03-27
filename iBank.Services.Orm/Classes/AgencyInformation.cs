
namespace iBank.Services.Orm.Classes
{
    public class AgencyInformation
    {
        public AgencyInformation()
        {
            Agency = string.Empty;
            DatabaseName = string.Empty;
            Active = false;
            BcActive = false;
            TimeZoneOffset = 0;
        }
        public string Agency { get; set; }
        public string DatabaseName { get; set; }
        public bool Active { get; set; }
        public bool BcActive { get; set; }
        public int TimeZoneOffset { get; set; }
    }
}
