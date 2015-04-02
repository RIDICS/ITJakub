using System.Collections.ObjectModel;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class TaskFinishedViewModel
    {
        public bool IsFinished { get; set; }

        public ObservableCollection<UserResultViewModel> ResultList { get; set; }
    }
}
