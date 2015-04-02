using System;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.SynchronizedReading.View.Converter
{
    public class RangeToZoomFactorConverter : IValueConverter
    {
        private const double Base = 1.1;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(double))
                throw new InvalidOperationException("The target must be a double");

            var percent = (double) value;
            return Math.Pow(Base, percent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(double))
                throw new InvalidOperationException("The target must be a double");

            return Math.Log((double) value, Base);
        }
    }
}