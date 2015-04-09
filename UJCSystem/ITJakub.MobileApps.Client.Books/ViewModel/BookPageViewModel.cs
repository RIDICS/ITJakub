using Windows.UI.Xaml.Media;

namespace ITJakub.MobileApps.Client.Books.ViewModel
{
    public class BookPageViewModel
    {
        public BookViewModel BookInfo { get; set; }

        public string PageName { get; set; }

        public int PagePosition { get; set; }

        public string RtfText { get; set; }

        public ImageSource PagePhoto { get; set; }
    }
}