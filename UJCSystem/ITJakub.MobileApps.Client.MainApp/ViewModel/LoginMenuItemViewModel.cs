using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Core.Manager;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class LoginMenuItemViewModel
    {
        public string Name { get; set; }
        public BitmapImage Icon { get; set; }
        public LoginProvider LoginProvider { get; set; }
    }
}