using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Fillwords2.DataService;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel.Data;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Fillwords2.ViewModel
{
    public class FillwordsTaskPreviewViewModel : TaskPreviewBaseViewModel
    {
        private readonly FillwordsDataService m_dataService;
        private ObservableCollection<SimpleWordOptionsViewModel> m_taskOptionsList;
        private string m_taskDocumentRtf;

        public FillwordsTaskPreviewViewModel(FillwordsDataService dataService)
        {
            m_dataService = dataService;
        }

        public string TaskDocumentRtf
        {
            get { return m_taskDocumentRtf; }
            set
            {
                m_taskDocumentRtf = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<SimpleWordOptionsViewModel> TaskOptionsList
        {
            get { return m_taskOptionsList; }
            set
            {
                m_taskOptionsList = value;
                RaisePropertyChanged();
            }
        }

        public override void ShowTask(string data)
        {
            m_dataService.SetTaskAndGetData(data, viewModel =>
            {
                TaskDocumentRtf = viewModel.DocumentRtf;
                TaskOptionsList = viewModel.Options;

                foreach (var optionsViewModel in TaskOptionsList)
                {
                    optionsViewModel.SelectedAnswer = optionsViewModel.CorrectAnswer;
                }
            });
        }
    }
}
