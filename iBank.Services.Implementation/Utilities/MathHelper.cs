using System;
using System.Linq;

namespace iBank.Services.Implementation.Utilities
{
    public static class MathHelper
    {
        public static decimal Round(decimal value, int digits)
        {
            return Math.Round(value, digits, MidpointRounding.AwayFromZero);
        }
        public static double Round(double value, int digits)
        {
            return Math.Round(value, digits, MidpointRounding.AwayFromZero);
        }
        public static decimal Round(decimal value)
        {
            return Round(value, 2);
        }
        public static double Round(double value)
        {
            return Round(value, 2);
        }

        public static int Max(int x, int y)
        {
            return Math.Max(x, y);
        }

        public static int Max(int x, int y, int z)
        {
            return Math.Max(x, Math.Max(y, z));
        }

        public static int Max(int w, int x, int y, int z)
        {
            return Math.Max(w, Math.Max(x, Math.Max(y, z)));
        }

        public static int Max(params int[] values)
        {
            return values.Max();
        }

        public static decimal Percent(decimal numerator, int denominator, int digits)
        {
            return Math.Round(numerator / denominator * 100, digits);
        }
        public static decimal Percent(double numerator, int denominator, int digits)
        {
            return Percent((decimal)numerator, denominator, digits);
        }

        public static decimal Percent(int numerator, int denominator, int digits)
        {
            return Percent((decimal)numerator, denominator, digits);
        }
    }
}
