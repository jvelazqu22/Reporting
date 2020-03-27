namespace Domain.Orm.Classes
{
    public class TripChangeCodeInformation
    {
        public int RecordNo { get; set; } = 0;
        public string LanguageCode { get; set; } = string.Empty;
        public int ChangeCode { get; set; } = 0;
        public string ChangeGroup { get; set; } = string.Empty;
        public string CodeDescription { get; set; } = string.Empty;
        public bool Active { get; set; } = false;
        public int Priority { get; set; }
    }
}
