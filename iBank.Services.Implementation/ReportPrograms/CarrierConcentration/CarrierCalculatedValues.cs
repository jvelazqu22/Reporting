namespace iBank.Services.Implementation.ReportPrograms.CarrierConcentration
{
    public class CarrierCalculatedValues
    {
        private readonly CarrierConcentrationCalculations _calc = new CarrierConcentrationCalculations();
        public decimal AverageFare { get; set; }
        public decimal CarrierPercentage { get; set; }
        public decimal CarrierAverage { get; set; }
        public decimal OtherPercentage { get; set; }
        public decimal OtherAverage { get; set; }
        public decimal CarrierSavings { get; set; }
        public decimal OtherSavings { get; set; }
        public decimal AvgSegmentDifference { get; set; }

        public CarrierCalculatedValues(decimal fare, int segments, int carrierOneSegs, decimal carrierOneFare)
        {
            SetValues(fare, segments, carrierOneSegs, carrierOneFare);
        }

        private void SetValues(decimal fare, int segments, int carrierOneSegs, decimal carrierOneFare)
        {
            if (segments != 0)
            {
                AverageFare = _calc.GetAvgFare(fare, segments);
                CarrierPercentage = _calc.GetCarrierPercentage(carrierOneSegs, segments);
                OtherPercentage = _calc.GetOtherPercentage(segments, carrierOneSegs);
            }

            if (carrierOneSegs != 0)
            {
                CarrierAverage = _calc.GetCarrierAverage(carrierOneFare, carrierOneSegs);
            }

            if (segments - carrierOneSegs != 0)
            {
                OtherAverage = _calc.GetOtherAverage(fare, carrierOneFare, segments, carrierOneSegs);
            }

            CarrierSavings = _calc.GetCarrierSavings(segments, carrierOneSegs, OtherAverage, CarrierAverage);
            OtherSavings = _calc.GetOtherSavings(carrierOneSegs, CarrierAverage, OtherAverage, segments);
            AvgSegmentDifference = _calc.GetAvgSegmentDifference(OtherAverage, CarrierAverage);
        }
    }
}
