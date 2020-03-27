namespace Domain.Orm.Classes
{
    public class DatabaseInformation
    {
        public string DatabaseName { get; set; } = string.Empty;
        public int TimeZoneOffset { get; set; } = 0;
        public string ServerName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
