namespace iBank.Services.Orm.Classes
{
    public class DatabaseInformation
    {
        public string DatabaseName { get; set; }
        public int TimeZoneOffset { get; set; }
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public DatabaseInformation()
        {
            DatabaseName = string.Empty;
            TimeZoneOffset = 0;
            ServerName = string.Empty;

            UserName = string.Empty;
            Password = string.Empty;
        }
    }
}
