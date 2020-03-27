namespace iBank.Services.Orm.Classes
{
    public class MasterAgencyInformation
    {
        public MasterAgencyInformation()
        {
            DatabaseName = string.Empty;
            ServerName = string.Empty;
            AgencyName = string.Empty;
            ReasonExclude = string.Empty;
            iBankVersion = string.Empty;
            ClientURL = string.Empty;
            SpecialUse1 = string.Empty;
            Address1 = string.Empty;
            Address2 = string.Empty;
            Address3 = string.Empty;
            Address4 = string.Empty;
        }
        public string DatabaseName { get; set; }
        public string ServerName { get; set; }
        public string AgencyName { get; set; }
        public string ReasonExclude { get; set; }
        public bool UseServiceFees { get; set; }
        public string iBankVersion { get; set; }
        public string ClientURL { get; set; }
        public string SpecialUse1 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
    }
}
