using System;
using System.Text;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.Shared.Converter
{
    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            var numValue = (double) value;

            if (parameter == null)
                return string.Format("{0:0.00}", numValue);

            var digitCount = (int) parameter;
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("{0:0.0");
            for (int i = 1; i < digitCount; i++)
            {
                stringBuilder.Append("0");
            }
            stringBuilder.Append("}");

            var formatString = stringBuilder.ToString();
            return string.Format(formatString, numValue);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}