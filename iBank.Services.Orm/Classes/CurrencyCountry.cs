namespace iBank.Services.Orm.Classes
{
    public class CurrencyCountry
    {
        public CurrencyCountry()
        {
            Recordno = 0;
            Country = string.Empty;
            CurCode = string.Empty;
            Symbol = string.Empty;
            LeftRight = string.Empty;
            Decimal = string.Empty;
            Thousands = string.Empty;
            Inactive = false;
            NumCurCode = string.Empty;
            CountryName = string.Empty;
            CountryCode = string.Empty;
        }

        public int Recordno { get; set; }
        public string Country { get; set; }
        public string CurCode { get; set; }
        public string Symbol { get; set; }
        public string LeftRight { get; set; }
        public string Decimal { get; set; }
        public string Thousands { get; set; }
        public bool Inactive { get; set; }
        public string NumCurCode { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
    }
}
