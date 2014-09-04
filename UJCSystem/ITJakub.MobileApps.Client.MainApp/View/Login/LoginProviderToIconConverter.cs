using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Client.MainApp.View.Login
{
    public class LoginProviderToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof (ImageSource))
                throw new InvalidOperationException("The target must be a ImageSource");

            return GetImageForAccount((AuthProvidersContract)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }

        private ImageSource GetImageForAccount(AuthProvidersContract value)
        {
            switch (value)
            {
                case AuthProvidersContract.Facebook:
                    return new BitmapImage(new Uri("ms-appx:///Icon/facebook-128.png"));
                case AuthProvidersContract.Google:
                    return new BitmapImage(new Uri("ms-appx:///Icon/google_plus-128.png"));
                case AuthProvidersContract.LiveId:
                    return new BitmapImage(new Uri("ms-appx:///Icon/windows8-128.png"));
                case AuthProvidersContract.ItJakub:
                    return new BitmapImage(new Uri("ms-appx:///Icon/LogoMobileApps_128x128.png"));
                default:
                    return new BitmapImage(new Uri("ms-appx:///Icon/login-128.png"));
            }
        }
    }
}