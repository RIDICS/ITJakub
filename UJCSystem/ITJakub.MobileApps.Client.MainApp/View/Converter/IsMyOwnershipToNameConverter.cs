using System;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.MainApp.View.Converter
{
    public class IsMyOwnershipToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            var isMe = (bool)value;
            return isMe ? "Moje" : "Ostatní autoři" ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}