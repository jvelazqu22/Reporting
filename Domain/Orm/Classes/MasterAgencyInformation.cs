namespace Domain.Orm.Classes
{
    public class MasterAgencyInformation
    {
        public string DatabaseName { get; set; } = string.Empty;
        public string ServerName { get; set; } = string.Empty;
        public string AgencyName { get; set; } = string.Empty;
        public string ReasonExclude { get; set; } = string.Empty;
        public bool UseServiceFees { get; set; }
        public string iBankVersion { get; set; } = string.Empty;
        public string ClientURL { get; set; } = string.Empty;
        public string SpecialUse1 { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Address3 { get; set; } = string.Empty;
        public string Address4 { get; set; } = string.Empty;
    }
}
