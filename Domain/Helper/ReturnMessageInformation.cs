namespace Domain.Helper
{
    public class ReturnMessageInformation
    {
        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; }

        public ReturnMessageInformation()
        {
            ReturnCode = 1;
            ReturnMessage = "";
        }
    }
}
