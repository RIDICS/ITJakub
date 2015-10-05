using System.Collections.ObjectModel;

namespace ITJakub.MobileApps.Client.Fillwords2.ViewModel.Data
{
    public class TaskFinishedViewModel
    {
        public bool IsFinished { get; set; }

        public ObservableCollection<UserResultViewModel> ResultList { get; set; }
    }
}
