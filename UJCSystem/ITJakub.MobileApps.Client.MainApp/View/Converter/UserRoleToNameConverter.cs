using System;
using Windows.UI.Xaml.Data;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Client.MainApp.View.Converter
{
    public class UserRoleToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            var userRole = (UserRoleContract)value;
            switch (userRole)
            {
                case UserRoleContract.Student:
                    return "Student";
                case UserRoleContract.Teacher:
                    return "Učitel";
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