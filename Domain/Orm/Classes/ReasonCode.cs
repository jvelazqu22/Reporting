namespace Domain.Orm.Classes
{
    public class ReasonCode
    {
        public string Agency { get; set; } = string.Empty;
        public string ReasCode { get; set; } = string.Empty;
        public string ReasDesc { get; set; } = string.Empty;
        public string ParentAcct { get; set; } = string.Empty;
        public int ReasSetNbr { get; set; } = 0;
        public string LangCode { get; set; } = string.Empty;
        public string LongDesc { get; set; } = string.Empty;
        public string ExtendDesc { get; set; } = string.Empty;
    }
}
