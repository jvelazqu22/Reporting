namespace Domain.Helper
{
    public class ProcessCaptionInformation
    {
        public ProcessCaptionInformation()
        {
            ProcessKey = 0;
            Caption = string.Empty;
            Usage = string.Empty;
        }
        public int ProcessKey { get; set; }
        public string Caption { get; set; }
        public string Usage { get; set; }
    }
}
