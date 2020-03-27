namespace Domain.Orm.Classes
{
    public class MasterSourceInformation
    {
        public MasterSourceInformation()
        {
            Agency = string.Empty;
            SourceAbbreviation = string.Empty;
            SourceDescription = string.Empty;
        }
        public string Agency { get; set; }
        public string SourceAbbreviation { get; set; }
        public string SourceDescription { get; set; }
    }
}
