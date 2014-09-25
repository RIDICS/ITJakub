using System;
using Windows.UI.Text;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.Shared.Converter
{
    public class HighlightMeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(FontWeight))
                throw new InvalidOperationException("The target must be a FontWeight");

            var isMe = (bool) value;
            return isMe ? FontWeights.Bold : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}