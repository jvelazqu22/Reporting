namespace iBank.IntegrationTests.TestTools
{
    public class ReportHandoffInformation
    {
        public ReportHandoffInformation()
        {
            ParmName = string.Empty;
            ParmValue = string.Empty;
            ParmInOut = "IN";
            LangCode = string.Empty;
        }
        public string ParmName { get; set; }
        public string ParmValue { get; set; }
        public string ParmInOut { get; set; }
        public string LangCode { get; set; }
    }
}
