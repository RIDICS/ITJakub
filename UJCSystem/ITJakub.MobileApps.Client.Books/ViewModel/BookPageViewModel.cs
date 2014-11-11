using System;
using Windows.UI.Xaml.Media.Imaging;

namespace ITJakub.MobileApps.Client.Books.ViewModel
{
    public class BookPageViewModel
    {
        private WeakReference<BitmapImage> m_pagePhoto;


        public BookViewModel BookInfo { get; set; }

        public string PageId { get; set; }

        public string RtfText { get; set; }

        public BitmapImage PagePhoto
        {
            get
            {
                BitmapImage image;
                if (m_pagePhoto != null && m_pagePhoto.TryGetTarget(out image))
                    return image;

                return null;
            }
            set
            {
                if (m_pagePhoto == null)
                    m_pagePhoto = new WeakReference<BitmapImage>(value);
                else
                    m_pagePhoto.SetTarget(value);
            }
        }
    }
}