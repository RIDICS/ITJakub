using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Hangman.DataService;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class HangmanTaskPreviewViewModel : TaskPreviewBaseViewModel
    {
        private readonly IHangmanDataService m_dataService;
        private ObservableCollection<TaskLevelDetailViewModel> m_taskLevelList;

        public HangmanTaskPreviewViewModel(IHangmanDataService dataService)
        {
            m_dataService = dataService;
        }

        public override void ShowTask(string data)
        {
            m_dataService.GetTaskDetail(data, taskLevelList =>
            {
                TaskLevelList = taskLevelList;
            });
        }

        public ObservableCollection<TaskLevelDetailViewModel> TaskLevelList
        {
            get { return m_taskLevelList; }
            set
            {
                m_taskLevelList = value;
                RaisePropertyChanged();
            }
        }
    }
}