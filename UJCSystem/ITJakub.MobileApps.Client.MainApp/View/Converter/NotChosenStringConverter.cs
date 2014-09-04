using System;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.MainApp.View.Converter
{
    public class NotChosenStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            var originalString = (string)value;

            return string.IsNullOrEmpty(originalString) ? "(Není zvoleno)" : originalString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}
