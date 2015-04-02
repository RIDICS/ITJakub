using System;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.MainApp.View.Converter
{
    public class GroupCountToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            var groupCount = (int)value;
            if (groupCount == 1)
            {
                return "1 skupina";
            }

            if (groupCount > 1 && groupCount < 5)
            {
                return string.Format("{0} skupiny", groupCount);
            }

            return string.Format("{0} skupin", groupCount);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}