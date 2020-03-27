namespace iBank.Services.Orm.Classes
{
    public class AdvancedColumnInformation
    {
        public AdvancedColumnInformation()
        {
            ColName = string.Empty;
            BigName = string.Empty;
            AdvancedColName = string.Empty;
            ColType = string.Empty;
            ColTable = string.Empty;
            LookupFunction = string.Empty;
            Usage = string.Empty;
        }
        public string ColName { get; set; }
        public string BigName { get; set; }
        public string AdvancedColName { get; set; }
        public string ColType { get; set; }
        public string ColTable { get; set; }
        public bool IsLookup { get; set; }
        public string LookupFunction { get; set; }
        public string Usage { get; set; }

    }
}
