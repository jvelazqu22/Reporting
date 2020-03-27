namespace Domain.Models.ReportPrograms.UserDefinedReport
{
    public class UserDefinedHeaders
    {
        public string HeaderOne { get; set; }
        public string HeaderTwo { get; set; }

        public UserDefinedHeaders()
        {
        }

        public UserDefinedHeaders(string headerOne, string headerTwo)
        {
            HeaderOne = headerOne;
            HeaderTwo = headerTwo;
        }
    }
}
