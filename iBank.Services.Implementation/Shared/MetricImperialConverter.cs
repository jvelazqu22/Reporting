using System;
using System.Collections.Generic;

using Domain.Interfaces;

namespace iBank.Services.Implementation.Shared
{
    public static class MetricImperialConverter
    {
        public static double ConvertMilesToKilometers(double miles)
        {
            return miles * 1.60944;
        }

        public static void ConvertMilesToKilometers<T>(IList<T> data, bool useCeilingToRound = true) where T : class, IMileage
        {
            foreach (var row in data)
            {
                row.Miles = useCeilingToRound
                                ? (int)Math.Ceiling(ConvertMilesToKilometers(row.Miles))
                                : (int)Math.Round(ConvertMilesToKilometers(row.Miles), 0);
            }
        }

        public static decimal ConvertPoundsToKilograms(decimal result, int precisionToRound)
        {
            return Math.Round(result * 0.45359237M, precisionToRound);
        }
    }
}
