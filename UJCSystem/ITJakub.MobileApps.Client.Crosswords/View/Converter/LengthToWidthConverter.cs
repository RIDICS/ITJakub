using System;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.Crosswords.View.Converter
{
    public class LengthToWidthConverter :IValueConverter
    {
        private const int KeyboardButtonWidth = 60;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(double))
                throw new InvalidOperationException("The target must be a double");

            var integer = (int) value;
            var multiplier = int.Parse(parameter.ToString());

            return integer*multiplier + 10 + KeyboardButtonWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}
