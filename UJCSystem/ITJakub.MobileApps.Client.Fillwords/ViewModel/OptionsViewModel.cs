using System.Collections.ObjectModel;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class OptionsViewModel
    {
        public int WordPosition { get; set; }
        
        public ObservableCollection<OptionViewModel> List { get; set; }

        public string CorrectAnswer { get; set; }
    }

    public class OptionViewModel
    {
        public string Word { get; set; }
    }
}