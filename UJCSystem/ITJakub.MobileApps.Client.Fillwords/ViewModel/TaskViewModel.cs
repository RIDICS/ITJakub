using System.Collections.ObjectModel;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class TaskViewModel
    {
        public string DocumentRtf { get; set; }

        public ObservableCollection<OptionsViewModel> Options { get; set; }
    }
}