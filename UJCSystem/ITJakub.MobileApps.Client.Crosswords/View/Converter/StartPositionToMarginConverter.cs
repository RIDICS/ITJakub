using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.Crosswords.View.Converter
{
    public class StartPositionToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(Thickness))
                throw new InvalidOperationException("The target must be a Thickness");

            var startPosition = (int) value;
            var marginValue = int.Parse(parameter.ToString());

            return new Thickness(startPosition*marginValue, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}
