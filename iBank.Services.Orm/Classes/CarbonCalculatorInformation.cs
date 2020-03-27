namespace iBank.Services.Orm.Classes
{
    public class CarbonCalculatorInformation
    {
        public CarbonCalculatorInformation()
        {
            Id = string.Empty;
            Name = string.Empty;
            Html = string.Empty;
            Link = string.Empty;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Html { get; set; }
        public string Link { get; set; }
    }
}
