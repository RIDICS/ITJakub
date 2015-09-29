using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.SynchronizedReading.View.Converter
{
    public class BooleanToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(double))
                throw new InvalidOperationException("The target must be a double");

            var parameterValue = System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);

            var isTrue = (bool)value;
            return isTrue ? parameterValue : 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}