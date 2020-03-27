namespace iBank.Services.Orm.Classes
{
    public class MetroInformation
    {
        public MetroInformation()
        {
            RecordNo = 0;
            MetroCode = string.Empty;
            MetroCity = string.Empty;
            MetroState = string.Empty;
            CountryCode = string.Empty;
        }

        public int RecordNo { get; set; }
        public string MetroCode { get; set; }
        public string MetroCity { get; set; }
        public string MetroState { get; set; }
        public string CountryCode { get; set; }
    }
}
