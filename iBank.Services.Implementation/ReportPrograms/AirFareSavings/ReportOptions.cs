using Domain.Helper;

namespace iBank.Services.Implementation.ReportPrograms.AirFareSavings
{
    public class ReportOptions
    {
        public UserBreaks UserBreaks { get; set; }
        public bool UsePageBreakHomeCountry { get; set; }
        public bool UseAccountBreak { get; set; }

        public bool UseBaseFare { get; set; }

        public bool UseDerivedSavingsCode { get; set; }

        public ReportOptions(UserBreaks breaks, bool usePageBreakHomeCountry, bool useAccountBreak, bool useBaseFare, bool useDerivedSavingsCode)
        {
            UserBreaks = breaks;
            UsePageBreakHomeCountry = usePageBreakHomeCountry;
            UseAccountBreak = useAccountBreak;
            UseBaseFare = useBaseFare;
            UseDerivedSavingsCode = useDerivedSavingsCode;
        }

    }
}
