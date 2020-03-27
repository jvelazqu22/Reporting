using System;

namespace Domain.Orm.Classes
{
    public class CurrencyConversionInformation
    {
        public DateTime CurrencyDate { get; set; } = DateTime.Now;
        public string CurrencyCode { get; set; } = "USD";
        public double USDFactor { get; set; }
    }
}
