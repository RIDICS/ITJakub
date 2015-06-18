using System;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.Books.View.Converter
{
    public class NoAuthorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            var stringValue = value as string;
            return string.IsNullOrEmpty(stringValue) ? "(Žádný autor)" : stringValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}