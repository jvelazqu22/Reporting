namespace Domain.Helper
{
    public class ConversionRates
    {
        public double SourceRate { get; set; } = 0;
        public double DestinationRate { get; set; } = 0;
        public bool SourceRateFound { get; set; } = false;
        public bool DestinationRateFound { get; set; } = false;
    }
}