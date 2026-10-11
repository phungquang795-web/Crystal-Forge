using System;

namespace CrystalForge.Utils
{
    public static class CrystalFormat
    {
        private static readonly string[] Suffixes = { "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc" };

        public static string Format(double value)
        {
            if (value < 0) return "-" + Format(-value);
            if (value < 1000.0)
            {
                return value < 10.0 && value > 0 ? value.ToString("F1") : Math.Floor(value).ToString("N0");
            }

            int exponent = (int)(Math.Log10(value) / 3.0);
            if (exponent >= Suffixes.Length)
            {
                exponent = Suffixes.Length - 1;
            }

            double divisor = Math.Pow(10, exponent * 3);
            double shortValue = value / divisor;

            return $"{shortValue:F2}{Suffixes[exponent]}";
        }

        public static string FormatRate(double ratePerSec)
        {
            return $"+{Format(ratePerSec)}/s";
        }
    }
}
