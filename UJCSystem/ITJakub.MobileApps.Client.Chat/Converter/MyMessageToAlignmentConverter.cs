using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.Chat.Converter
{
    public class MyMessageToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(HorizontalAlignment))
                throw new InvalidOperationException("The target must be a HorizontalAlignment");

            return (bool)value ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
