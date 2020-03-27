namespace Domain.Helper
{
    public class EffectsOutputInformation
    {
        public int EProfileNumber { get; set; }
        public int TradingPartnerNumber { get; set; }
        public bool DirectDelivery { get; set; }
        public string ProfileName { get; set; }
        public string FileNameMask { get; set; }
        public bool ZipOutput { get; set; }
        public string TradingPartnerName { get; set; }
        public string Outbox { get; set; }
    }
}
