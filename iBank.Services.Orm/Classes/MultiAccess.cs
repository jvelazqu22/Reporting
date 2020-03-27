namespace iBank.Services.Orm.Classes
{
    public class MultiAccess
    {
        public MultiAccess()
        {
            DBName = string.Empty;
            Agency = string.Empty;
        }
        public string DBName { get; set; }
        public string Agency { get; set; }
    }
}
