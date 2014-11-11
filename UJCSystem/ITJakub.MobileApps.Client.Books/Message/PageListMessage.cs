using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Books.ViewModel;

namespace ITJakub.MobileApps.Client.Books.Message
{
    public class PageListMessage
    {
        public ObservableCollection<BookPageViewModel> PageList { get; set; }
    }
}