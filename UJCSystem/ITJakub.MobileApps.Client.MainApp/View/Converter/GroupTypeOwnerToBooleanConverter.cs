using System;
using Windows.UI.Xaml.Data;
using ITJakub.MobileApps.Client.Core.Manager.Groups;

namespace ITJakub.MobileApps.Client.MainApp.View.Converter
{
    public class GroupTypeOwnerToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a bool");

            var groupType = (GroupType)value;

            return groupType == GroupType.Owner;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}