using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel.News;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.News
{
    public class NewsViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private bool m_loading;
        private ObservableCollection<SyndicationItemViewModel> m_news;
        private bool m_noNewsExist;

        public NewsViewModel(IDataService dataService)
        {
            m_dataService = dataService;
            News = new ObservableCollection<SyndicationItemViewModel>();
            LoadData();
        }

        public ObservableCollection<SyndicationItemViewModel> News
        {
            get { return m_news; }
            set
            {
                m_news = value;
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

        public bool NoNewsExist
        {
            get { return m_noNewsExist; }
            set
            {
                m_noNewsExist = value;
                RaisePropertyChanged();
            }
        }

        private async void LoadData()
        {
            Loading = true;
            try
            {
                var data = await m_dataService.GetAllNews();

                foreach (var item in data)
                {
                    News.Add(item);
                }

                NoNewsExist = News.Count == 0;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Loading = false;
            }


            //await Task.Run(() =>
            //{
            //    foreach (var item in data)
            //    {                    
            //        DispatcherHelper.CheckBeginInvokeOnUI(()=>News.Add(item));
            //    }
            //});
        }
    }
}