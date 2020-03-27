namespace Domain.Orm.Classes
{
    public class InternationalSettingsInformation
    {
        public string DateFormat { get; set; } = "AMERICAN";
        public string DateMark { get; set; } = "/";
        public string Symbol { get; set; } = "$";
        public string Decimal { get; set; } = ".";
        public string Position { get; set; } = "L";
        public string Thousand { get; set; } = ",";
        public string DateDisplay { get; set; } = "MM/DD/YYYY";
        public string CountryDescription { get; set; } = "United States";
    }
}
