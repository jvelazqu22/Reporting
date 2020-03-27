namespace Domain.Models.Repository.Api
{
    public class ServiceRequest
    {
        public string ServiceType { get; set; }
        public string ExportSubDirectory { get; set; } = string.Empty;

        public UserInfo UserInfo { get; set; }

        public ServiceBody ServiceBody { get; set; }
    }
}

