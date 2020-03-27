using System;

namespace Domain.Helper
{
    public class ErrorInformation
    {
        public ErrorInformation()
        {
            Exception = new Exception();
            Agency = string.Empty;
            ErrorMessage = string.Empty;
            Version = string.Empty;
            ErrorDate = DateTime.Now;
            ErrorNumber = -1;
        }
        public Exception Exception { get; set; }
        public string Agency { get; set; }
        public int UserNumber { get; set; }
        public string ErrorMessage { get; set; }
        public string Version { get; set; }
        public int ServerNumber { get; set; }
        public string ServerName { get; set; }
        public DateTime ErrorDate { get; set; }
        public short ErrorNumber { get; set; }
        public string ErrorProgram { get; set; }
        public short LineNumber { get; set; }

    }
}
