using System;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.Books.View.Converter
{
    public class RangeToZoomPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            var percent = (double) value;
            return string.Format("{0:#%}",  Math.Pow(1.1, percent));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}