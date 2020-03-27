namespace Domain.Models.Repository.Api
{
    public class Result
    {
        public int StatusID { get; set; }
        public string ReportName { get; set; }
        public string ReportUrl { get; set; }
        public string ErrorMsg { get; set; }
        public ServiceRequest ServiceRequest { get; set; }
        public PrintResponse PrintResponse { get; set; }
    }
}
