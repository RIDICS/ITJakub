using System;
using Windows.UI.Xaml.Data;
using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.SynchronizedReading.View.Converter
{
    public class UserInfoToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            var userInfo = value as UserInfo;

            return userInfo == null || userInfo.LastName == null ? "Nikdo" : string.Format("{0} {1}", userInfo.FirstName, userInfo.LastName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}
