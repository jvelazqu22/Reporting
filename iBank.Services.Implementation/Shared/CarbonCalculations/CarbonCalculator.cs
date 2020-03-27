using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Domain.Interfaces;
using Domain.Services;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.Shared.CarbonCalculations
{
    public class CarbonCalculator
    {
        private readonly CarbonLookup _lookup;

        private static readonly List<string> _airClassCategories = new List<string> { "F", "B", "P" };
        private static readonly List<string> _fourByFours = new List<string> { "CDAR", "CFAR", "CRAR", "FFAR", "IFAR", "SFAR" };
        private static readonly List<string> _sportsCars = new List<string> { "PFAR", "PSAN", "PSAR" };

        public CarbonCalculator()
        {
            _lookup = new CarbonLookup();
        }

        public CarbonCalculator(ICacheService cache, ICisMastersDataStore store)
        {
            _lookup = new CarbonLookup(cache, store);
        }
        
        public void SetHotelCarbon<T>(IList<T> rawData, bool useMetric, bool isPreview) where T : class, ICarbonCalculationsHotel
        {
            foreach (var row in rawData)
            {
                var plusmin = isPreview ? 1 : row.HPlusMin;

                var result = _lookup.GetHotelStayCarbon(row.Nights, row.Rooms, plusmin);

                row.HotelCo2 = useMetric ? MetricImperialConverter.ConvertPoundsToKilograms(result, 0) : result;
            }
        }
        
        public void SetCarCarbon<T>(IList<T> rawData, bool useMetric, bool isPreview) where T : class, ICarbonCalculationsCar
        {
            foreach (var row in rawData)
            {
                var plusmin = isPreview ? 1 : row.CPlusMin;
                
                var result = _lookup.GetEngineCarbon(100, row.Days, plusmin, IsLargeEngine(row.CarType));
                
                row.CarCo2 = useMetric ? MetricImperialConverter.ConvertPoundsToKilograms(result, 1) : result;
            }
        }

        public void SetAirCarbon<T>(IList<T> rawData, bool useMetric, string carbCalculator) where T : class, ICarbonCalculations
        {
            if (!rawData.Any() || string.IsNullOrEmpty(carbCalculator.Trim())) return;

            var tType = rawData[0].GetType();
            var tProperties = new List<PropertyInfo>(tType.GetProperties());
            var plusminProperty = GetProperty(tProperties, "plusMin");
            var rplusminProperty = GetProperty(tProperties, "rPlusmin");
            var modeProperty = GetProperty(tProperties, "mode");

            foreach (var row in rawData)
            {
                var rPlusMin = 1;
                if (rplusminProperty != null)
                {
                    rPlusMin = rplusminProperty.GetValue(row, null).GetIntSafe();
                }
                else if (plusminProperty != null)
                {
                    rPlusMin = plusminProperty.GetValue(row, null).GetIntSafe();
                }

                var miles = row.Miles.GetIntSafe();
                var classCat = GetClassCategory(row, carbCalculator);

                SetCarbonValues(row, miles * rPlusMin, row.DitCode, classCat, useMetric);

                if (modeProperty != null)
                {
                    var mode = (string)modeProperty.GetValue(row, null) ?? string.Empty;
                    if (IsRail(mode))
                    {
                        row.AirCo2 = row.AltRailCo2;
                        row.AltRailCo2 = 0m;
                    }
                }
            }
        }

        private static string GetClassCategory<T>(T row, string carbonCalculator) where T : class, ICarbonCalculations
        {
            var classCat = row.ClassCat?.Trim() ?? string.Empty;
            if ((carbonCalculator.EqualsIgnoreCase("NEUTRAL") || carbonCalculator.EqualsIgnoreCase("OFFSETTERS")) 
                && !_airClassCategories.Contains(classCat.Trim()))
            {
                classCat = "E";
            }

            return classCat;
        }

        private void SetCarbonValues<T>(T row, int miles, string ditCode, string classCat, bool useMetric) where T : class, ICarbonCalculations
        {
            classCat = classCat.Trim();
            decimal lbsPerMile;
            string haulType;

            if (Math.Abs(miles) <= _lookup.Carbon.ShortHaulMileageLimit)
            {
                lbsPerMile = _lookup.Carbon.ShortHaulLbsPerMileRate;
                haulType = _lookup.Carbon.ShortHaulDesignation;
            }
            else if (Math.Abs(miles) <= _lookup.Carbon.MediumHaulMileageLimit)
            {
                haulType = _lookup.Carbon.MediumHaulDesignation;
                lbsPerMile = IsEconomyOrPremiumEconomy(classCat)
                                 ? _lookup.Carbon.MediumHaulEconomyLbsPerMileRate
                                 : _lookup.Carbon.MediumHaulDefaultLbsPerMileRate;
            }
            else
            {
                haulType = _lookup.Carbon.LongHaulDesignation;
                switch (classCat)
                {
                    case "E":
                    case "P":
                        lbsPerMile = _lookup.Carbon.LongHaulEconomyLbsPerMileRate;
                        break;
                    case "B":
                        lbsPerMile = _lookup.Carbon.LongHaulBusinessLbsPerMileRate;
                        break;
                    default:
                        lbsPerMile = _lookup.Carbon.LongHaulDefaultLbsPerMileRate;
                        break;
                }
            }
            
            var c02Pounds = _lookup.GetCarbonPounds(lbsPerMile, miles);
            var altCarC02 = _lookup.GetAltCarCarbon(miles);
            var altRailC02 = _lookup.GetAltRailCarbon(miles, ditCode);

            row.AirCo2 = useMetric ? MetricImperialConverter.ConvertPoundsToKilograms(c02Pounds, 1) : c02Pounds;
            row.AltCarCo2 = useMetric ? MetricImperialConverter.ConvertPoundsToKilograms(altCarC02, 1) : altCarC02;
            row.AltRailCo2 = useMetric ? MetricImperialConverter.ConvertPoundsToKilograms(altRailC02, 1) : altRailC02;
            row.HaulType = haulType;
        }

        private static PropertyInfo GetProperty(List<PropertyInfo> properties, string propertyName)
        {
            return properties.FirstOrDefault(x => x.Name.EqualsIgnoreCase(propertyName));
        }

        private static bool IsLargeEngine(string carType)
        {
            return _fourByFours.Contains(carType.Trim()) || _sportsCars.Contains(carType.Trim()) || carType.Left(1).EqualsIgnoreCase("P");
        }

        private static bool IsEconomyOrPremiumEconomy(string classCategory)
        {
            return classCategory.EqualsIgnoreCase("E") || classCategory.EqualsIgnoreCase("P");
        }

        private static bool IsRail(string mode)
        {
            return mode.EqualsIgnoreCase("R");
        }
    }
}
