using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;

namespace ITJakub.MobileApps.Client.MainApp.View.Login
{
    public class LoginProviderToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof (ImageSource))
                throw new InvalidOperationException("The target must be a ImageSource");

            return GetImageForAccount((LoginProviderType) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }

        private ImageSource GetImageForAccount(LoginProviderType value)
        {
            switch (value)
            {
                case LoginProviderType.Facebook:
                    return new BitmapImage(new Uri("ms-appx:///Icon/facebook-128.png"));
                case LoginProviderType.Google:
                    return new BitmapImage(new Uri("ms-appx:///Icon/google_plus-128.png"));
                case LoginProviderType.LiveId:
                    return new BitmapImage(new Uri("ms-appx:///Icon/windows8-128.png"));
                default:
                    return new BitmapImage(new Uri("ms-appx:///Icon/login-128.png"));
            }
        }
    }
}