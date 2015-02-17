using Windows.UI.Xaml.Media.Imaging;

namespace ITJakub.MobileApps.Client.SynchronizedReading.ViewModel
{
    public class PageViewModel
    {
        public string PageId { get; set; }
    }

    public class PageDetailViewModel : PageViewModel
    {
        public string DocumentRtf { get; set; }

        public BitmapImage Photo { get; set; }
    }
}