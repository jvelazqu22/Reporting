using System;

using Domain.Helper;

namespace Domain.Models.ReportPrograms.CreditCardDetail
{
    [Exportable]
    public class FinalData
    {
        public string Cardnum { get; set; } = string.Empty;
        public string Refnbr { get; set; } = string.Empty;
        public DateTime Postdate { get; set; } = DateTime.MinValue;
        public DateTime Trandate { get; set; } = DateTime.MinValue;
        public string Purchtype { get; set; } = string.Empty;
        public string Merchname { get; set; } = string.Empty;
        public string Merchaddr1 { get; set; } = string.Empty;
        public string Merchcity { get; set; } = string.Empty;
        public string Merchstate { get; set; } = string.Empty;
        public string Merchsic { get; set; } = string.Empty;
        public decimal Transamt { get; set; } = 0m;
        public decimal Taxamt { get; set; } = 0m;

    }
}
