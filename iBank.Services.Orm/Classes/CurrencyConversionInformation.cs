using System;

namespace iBank.Services.Orm.Classes
{
    public class CurrencyConversionInformation
    {
        public CurrencyConversionInformation()
        {
            CurrencyCode = "USD";
            CurrencyDate = DateTime.Now;
        }
        public DateTime CurrencyDate { get; set; }
        public string CurrencyCode { get; set; }
        public double USDFactor { get; set; }
    }
}
