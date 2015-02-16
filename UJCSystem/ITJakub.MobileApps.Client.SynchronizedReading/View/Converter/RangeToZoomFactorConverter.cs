using System;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.SynchronizedReading.View.Converter
{
    public class RangeToZoomFactorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(double))
                throw new InvalidOperationException("The target must be a double");

            var percent = (double) value;
            return Math.Pow(1.1, percent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}