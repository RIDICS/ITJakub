using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Core.Service;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.News
{
    public class NewsViewModel:ViewModelBase
    {
        private readonly IDataService m_dataService;
        private bool m_loading;
        public ObservableCollection<string> News { get; set; }

        public NewsViewModel(IDataService dataService)
        {
            m_dataService = dataService;
            News = new ObservableCollection<string> {"Byla zveřejněna nová kniha",
            "Nové ikony pro hangmana",
            "Transliterace pro Jungmanův slovník započala. adasdasafafasfas",
            "Dostupné vyšší rozlišení obrázků pro elektronický staročeský slovník."};

            LoadData();
        }

        private async void LoadData()
        {
            Loading = true;
            var data = await m_dataService.GetAllNews();

            Loading = false;
            News = data;

        }

        public bool Loading
        {
            get { return m_loading; }
            set { m_loading = value;RaisePropertyChanged(); }
        }
    }
}
