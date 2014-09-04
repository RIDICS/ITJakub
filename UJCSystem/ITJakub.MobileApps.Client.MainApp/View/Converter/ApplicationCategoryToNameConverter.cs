using System;
using Windows.UI.Xaml.Data;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.MainApp.View.Converter
{
    public class ApplicationCategoryToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            var groupType = (ApplicationCategory)value;
            switch (groupType)
            {
                case ApplicationCategory.Education:
                    return "Výuka:";
                case ApplicationCategory.Game:
                    return "Hry:";
                case ApplicationCategory.Other:
                    return "Ostatní:";
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