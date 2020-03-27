namespace Domain.Orm.Classes
{
    public class CurrencyCountry
    {
        public int Recordno { get; set; } = 0;
        public string Country { get; set; } = string.Empty;
        public string CurCode { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string LeftRight { get; set; } = string.Empty;
        public string Decimal { get; set; } = string.Empty;
        public string Thousands { get; set; } = string.Empty;
        public bool Inactive { get; set; } = false;
        public string NumCurCode { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
    }
}
