using System.Collections.Generic;

using Domain.Constants;

using iBank.Entities.CISMasterEntities;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.Shared.CarbonCalculations
{
    public class CarbonValue
    {
        public decimal UpliftFactor { get; }
        public decimal LargeEngineRate { get; }
        public decimal MediumEngineRate { get; }
        public decimal LbsPerHotelNightRate { get; }
        public decimal AltCarRate { get; }
        public decimal AltRailDomesticRate { get; }
        public decimal AltRailIntlRate { get; }
        public int ShortHaulMileageLimit { get; }
        public decimal ShortHaulLbsPerMileRate { get; }
        public decimal MediumHaulMileageLimit { get; }
        public decimal MediumHaulEconomyLbsPerMileRate { get; }
        public decimal MediumHaulDefaultLbsPerMileRate { get; }
        public decimal LongHaulEconomyLbsPerMileRate { get; }
        public decimal LongHaulBusinessLbsPerMileRate { get; }
        public decimal LongHaulDefaultLbsPerMileRate { get; }
        public string ShortHaulDesignation { get; }
        public string MediumHaulDesignation { get; }
        public string LongHaulDesignation { get; }

        public CarbonValue(IList<CarbonCalculationRate> rates, IList<CarbonCalculationHaul> haulValues)
        {
            //rates
            UpliftFactor = rates.GetRate(CarbonCalculationKeys.UPLIFT_FACTOR);
            LargeEngineRate = rates.GetRate(CarbonCalculationKeys.LARGE_ENGINE_RATE);
            MediumEngineRate = rates.GetRate(CarbonCalculationKeys.MEDIUM_ENGINE_RATE);
            LbsPerHotelNightRate = rates.GetRate(CarbonCalculationKeys.LBS_PER_HOTEL_NIGHT_RATE);
            AltCarRate = rates.GetRate(CarbonCalculationKeys.ALT_CAR_RATE);
            AltRailDomesticRate = rates.GetRate(CarbonCalculationKeys.ALT_RAIL_DOMESTIC_RATE);
            AltRailIntlRate = rates.GetRate(CarbonCalculationKeys.ALT_RAIL_INTL_RATE);
            ShortHaulLbsPerMileRate = rates.GetRate(CarbonCalculationKeys.SHORT_HAUL_LBS_PER_MILE_RATE);
            MediumHaulEconomyLbsPerMileRate = rates.GetRate(CarbonCalculationKeys.MED_HAUL_ECONOMY_LBS_PER_MILE_RATE);
            MediumHaulDefaultLbsPerMileRate = rates.GetRate(CarbonCalculationKeys.MED_HAUL_DEFAULT_LBS_PER_MILE_RATE);
            LongHaulEconomyLbsPerMileRate = rates.GetRate(CarbonCalculationKeys.LONG_HAUL_ECONOMY_LBS_PER_MILE_RATE);
            LongHaulBusinessLbsPerMileRate = rates.GetRate(CarbonCalculationKeys.LONG_HAUL_BUSINESS_LBS_PER_MILE_RATE);
            LongHaulDefaultLbsPerMileRate = rates.GetRate(CarbonCalculationKeys.LONG_HAUL_DEFAULT_LBS_PER_MILE_RATE);

            //hauls
            var shortHaul = haulValues.GetHaul(CarbonCalculationKeys.SHORT_HAUL);
            ShortHaulDesignation = shortHaul.Abbreviation;
            ShortHaulMileageLimit = shortHaul.MileageLimit;

            var medHaul = haulValues.GetHaul(CarbonCalculationKeys.MEDIUM_HAUL);
            MediumHaulDesignation = medHaul.Abbreviation;
            MediumHaulMileageLimit = medHaul.MileageLimit;

            var longHaul = haulValues.GetHaul(CarbonCalculationKeys.LONG_HAUL);
            LongHaulDesignation = longHaul.Abbreviation;
        }
    }
}
