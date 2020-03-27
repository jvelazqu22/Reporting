namespace Domain.Helper
{
    public class TicketInfo
    {
        public TicketInfo()
        {
            Ticket = string.Empty;
            Invoice = string.Empty;
            Valcarr = string.Empty;
            MoneyType = string.Empty;
        }
        public int RecKey { get; set; }
        public string Ticket { get; set; }
        public string Invoice { get; set; }
        public string Valcarr { get; set; }
        public decimal AirChg { get; set; }
        public string MoneyType { get; set; }
    }
}
