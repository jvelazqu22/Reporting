
namespace Domain.Models.ReportPrograms.ValidatingCarrierReport
{
    public class FinalData
    {

        public FinalData()
        {
            Valcarr = string.Empty;
            Carrdesc = string.Empty;
            Transacts = 0;
            Tickets = 0m;
            Refunds = 0m;
            Net_trips = 0m;
            Commission = 0m;
            Invoiceamt = 0m;
            Creditamt = 0m;
            Netvolume = 0m;
        }
        public string Valcarr { get; set; }
        public string Carrdesc { get; set; }
        public int Transacts { get; set; }
        public decimal Tickets { get; set; }
        public decimal Refunds { get; set; }
        public decimal Net_trips { get; set; }
        public decimal Commission { get; set; }
        public decimal Invoiceamt { get; set; }
        public decimal Creditamt { get; set; }
        public decimal Netvolume { get; set; }

    }
}
