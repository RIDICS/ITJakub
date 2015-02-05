using System;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.MainApp.View.Converter
{
    public class MessageCountToNotificationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            string messageString;
            int messageCount = (int)value;

            if (messageCount == 1)
            {
                messageString = "jednu novou zprávu";
            }
            else if (messageCount > 1 && messageCount < 5)
            {
                messageString = string.Format("{0} nové zprávy", messageCount);
            }
            else
            {
                messageString = string.Format("{0} nových zpráv", messageCount);
            }

            return string.Format("V chatu máte {0}.", messageString);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}