namespace iBank.Services.Orm.Classes
{
    public class ReasonCode
    {
        public ReasonCode()
        {
            Agency = string.Empty;
            ReasCode = string.Empty;
            ReasDesc = string.Empty;
            ParentAcct = string.Empty;
            ReasSetNbr = 0;
            LangCode = string.Empty;
            LongDesc = string.Empty;
            ExtendDesc = string.Empty;
        }
        public string Agency { get; set; }
        public string ReasCode { get; set; }
        public string ReasDesc { get; set; }
        public string ParentAcct { get; set; }
        public int ReasSetNbr { get; set; }
        public string LangCode { get; set; }
        public string LongDesc { get; set; }
        public string ExtendDesc { get; set; }
    }

}
