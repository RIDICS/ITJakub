using System;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.Chat.Converter
{
    public class MessageDateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a HorizontalAlignment");

            if (!(value is DateTime))
                return string.Empty;

            var dateTime = (DateTime) value;
            dateTime = dateTime.ToLocalTime();
            return dateTime.ToString("H:mm:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}