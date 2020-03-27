namespace iBank.Services.Orm.Classes
{
    public class InternationalSettingsInformation
    {
        public InternationalSettingsInformation()
        {
            DateFormat = "AMERICAN";
            DateMark = "/";
            Symbol = "$";
            Decimal = ".";
            Position = "L";
            Thousand = ",";
            DateDisplay = "MM/DD/YYYY";
            CountryDescription = "United States";
        }
        public string DateFormat { get; set; }
        public string DateMark { get; set; }
        public string Symbol { get; set; }
        public string Decimal { get; set; }
        public string Position { get; set; }
        public string Thousand { get; set; }
        public string DateDisplay { get; set; }
        public string CountryDescription { get; set; }
        
    }
}
