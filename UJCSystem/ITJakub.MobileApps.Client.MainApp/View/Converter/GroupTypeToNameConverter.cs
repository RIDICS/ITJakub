using System;
using Windows.UI.Xaml.Data;
using ITJakub.MobileApps.Client.Core.Manager;

namespace ITJakub.MobileApps.Client.MainApp.View.Converter
{
    public class GroupTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            var groupType = (GroupType) value;
            switch (groupType)
            {
                case GroupType.Member:
                    return "Èlenství ve skupinách:";
                case GroupType.Owner:
                    return "Moje skupiny:";
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}