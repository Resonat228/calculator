using System;
using System.Windows.Forms;

namespace CreditsCalc
{
    public static class ConvertExtensions
    {
        public static string DoubleToString(this double value)
        {
            return value.ToString("N2");
        }

        public static double ToDouble(this object value)
        {
            if (double.TryParse(value.ToString(), out var result))
            {
                return result;
            }
            MessageBox.Show($"Convert Exeption: can't convert {value}");
            return default;

        }
        
        public static int ToInt32(this object value)
        {
            if (int.TryParse(value.ToString(), out var result))
            {
                return result;
            }
            MessageBox.Show($"Convert Exeption: can't convert {value}");
            return default;

        }
    }
}