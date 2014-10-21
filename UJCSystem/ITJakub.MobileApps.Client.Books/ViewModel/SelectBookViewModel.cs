using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Books.Message;
using ITJakub.MobileApps.Client.Books.Service;
using ITJakub.MobileApps.Client.Books.View;

namespace ITJakub.MobileApps.Client.Books.ViewModel
{
    public class SelectBookViewModel : ViewModelBase
    {
        private readonly DataService m_dataService;
        private readonly NavigationService m_navigationService;
        private ObservableCollection<BookViewModel> m_bookList;
        private bool m_noBookFound;
        private bool m_loading;
        private object m_searchFilterSelection;

        public SelectBookViewModel(DataService dataService, NavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;

            GoBackCommand = new RelayCommand(navigationService.GoBack);
            BookClickCommand = new RelayCommand<ItemClickEventArgs>(BookClick);
            BookList = new ObservableCollection<BookViewModel>
            {
                new BookViewModel
                {
                    Author = "Nějaký Autor",
                    Name = "Úžasná kniha",
                    Year = 1800
                },
                new BookViewModel
                {
                    Author = "Nějaký Autor",
                    Name = "Úžasná kniha",
                    Year = 1800
                },
                new BookViewModel
                {
                    Author = "Nějaký Autor",
                    Name = "Úžasná kniha",
                    Year = 1800
                },
                new BookViewModel
                {
                    Author = "Nějaký Autor",
                    Name = "Úžasná kniha",
                    Year = 1800
                },
            };
        }
        
        public RelayCommand GoBackCommand { get; private set; }
        
        public RelayCommand<ItemClickEventArgs> BookClickCommand { get; private set; }
        
        public ObservableCollection<BookViewModel> BookList
        {
            get { return m_bookList; }
            set
            {
                m_bookList = value;
                RaisePropertyChanged();
                NoBookFound = m_bookList.Count == 0;
            }
        }

        public bool NoBookFound
        {
            get { return m_noBookFound; }
            set
            {
                m_noBookFound = value;
                RaisePropertyChanged();
            }
        }

        public bool Loading
        {
            get { return m_loading; }
            set
            {
                m_loading = value;
                RaisePropertyChanged();
            }
        }

        private void BookClick(ItemClickEventArgs args)
        {
            var book = args.ClickedItem as BookViewModel;
            if (book == null)
                return;
            
            m_navigationService.Navigate(typeof(SelectPageView));
            MessengerInstance.Send(new SelectedBookMessage {Book = book});
        }
    }
}
