using Windows.UI.Xaml.Media.Imaging;

namespace ITJakub.MobileApps.Client.Books.ViewModel
{
    public class BookPageViewModel
    {
        public BookViewModel BookInfo { get; set; }

        public string PageId { get; set; }
        
        public BitmapImage PagePhoto { get; set; }

        public string RtfText { get; set; }
    }
}