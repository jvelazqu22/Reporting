namespace iBank.Services.Orm.Classes
{
    public class EmailLogoPathInfo
    {
        public string PhysicalPath { get; set; }
        public string UrlPath { get; set; }

        public EmailLogoPathInfo()
        {
            PhysicalPath = "";
            UrlPath = "";
        }
    }
}
