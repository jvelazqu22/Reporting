namespace iBank.Services.Orm.Classes
{
    public class TripChangeCodeInformation
    {
        public TripChangeCodeInformation()
        {
            RecordNo = 0;
            LanguageCode = string.Empty;
            ChangeCode = 0;
            ChangeGroup = string.Empty;
            CodeDescription = string.Empty;
            Active = false;
        }
        public int RecordNo { get; set; }
        public string LanguageCode { get; set; }
        public int ChangeCode { get; set; }
        public string ChangeGroup { get; set; }
        public string CodeDescription { get; set; }
        public bool Active { get; set; }
        public int Priority { get; set; }
    }
}
