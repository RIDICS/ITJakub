using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Hangman.ViewModel;

namespace ITJakub.MobileApps.Client.Hangman
{
    public class GameOverViewModel : ViewModelBase
    {
        public ObservableCollection<ProgressInfoViewModel> PlayerRanking { get; set; }


    }

    public class PlayerRankViewModel : ProgressInfoViewModel
    {
    }
}