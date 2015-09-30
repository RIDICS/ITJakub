using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.SynchronizedReading.View.Converter
{
    public class BooleanToGridVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(GridLength))
                throw new InvalidOperationException("The target must be a GridLength");

            var isVisible = (bool) value;

            return isVisible ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a bool");

            var gridLenghtValue = (GridLength) value;

            return gridLenghtValue.Value > 0;
        }
    }
}