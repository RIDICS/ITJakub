using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class OptionsViewModel
    {
        public int WordPosition { get; set; }

        public ObservableCollection<OptionViewModel> List { get; set; } 
    }

    public class OptionViewModel
    {
        public string Word { get; set; }

        public RelayCommand DeleteCommand { get; set; }
    }
}