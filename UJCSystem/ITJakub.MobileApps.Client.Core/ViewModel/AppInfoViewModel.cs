using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Core.ViewModel
{
    public class AppInfoViewModel
    {
        public string Name { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public BitmapImage Icon { get; set; }
        public ApplicationCategory ApplicationCategory { get; set; }
    }
}