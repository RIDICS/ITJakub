using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Books.ViewModel;

namespace ITJakub.MobileApps.Client.Books.Message
{
    public class PageListMessage
    {
        public PageListMessage(ObservableCollection<PageViewModel> pageList)
        {
            PageList = pageList;
        }

        public ObservableCollection<PageViewModel> PageList { get; set; }
    }
}