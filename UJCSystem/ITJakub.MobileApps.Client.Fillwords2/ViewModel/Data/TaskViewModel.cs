using System.Collections.ObjectModel;

namespace ITJakub.MobileApps.Client.Fillwords2.ViewModel.Data
{
    public class TaskViewModel
    {
        public string DocumentRtf { get; set; }

        public ObservableCollection<SimpleWordOptionsViewModel> Options { get; set; }
    }
}