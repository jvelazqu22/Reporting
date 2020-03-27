namespace Domain.Orm.Classes
{
    public class AdvancedColumnInformation
    {
        public string ColName { get; set; } = string.Empty;
        public string BigName { get; set; } = string.Empty;
        public string AdvancedColName { get; set; } = string.Empty;
        public string ColType { get; set; } = string.Empty;
        public string ColTable { get; set; } = string.Empty;
        public bool IsLookup { get; set; }
        public string LookupFunction { get; set; } = string.Empty;
        public string Usage { get; set; } = string.Empty;
    }
}
