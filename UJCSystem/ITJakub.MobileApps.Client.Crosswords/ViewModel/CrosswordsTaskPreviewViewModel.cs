using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Crosswords.DataService;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class CrosswordsTaskPreviewViewModel : TaskPreviewBaseViewModel
    {
        private readonly ICrosswordsDataService m_dataService;
        private ObservableCollection<CrosswordRowViewModel> m_crosswordRows;

        public CrosswordsTaskPreviewViewModel(ICrosswordsDataService dataService)
        {
            m_dataService = dataService;
        }

        public override void ShowTask(string data)
        {
            m_dataService.SetTaskAndGetConfiguration(data, rows =>
            {
                CrosswordRows = rows;
            }, true);
        }

        public ObservableCollection<CrosswordRowViewModel> CrosswordRows
        {
            get { return m_crosswordRows; }
            set
            {
                m_crosswordRows = value;
                RaisePropertyChanged();
            }
        }
    }
}