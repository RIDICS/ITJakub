using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Crosswords.ViewModel.Comparer;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class PlayerRankingViewModel : ViewModelBase
    {
        private bool m_isEnd;
        private ObservableCollection<ProgressViewModel> m_playerRanking;

        public PlayerRankingViewModel()
        {
            PlayerRanking = new ObservableCollection<ProgressViewModel>();
        }

        public ObservableCollection<ProgressViewModel> PlayerRanking
        {
            get { return m_playerRanking; }
            private set
            {
                m_playerRanking = value;
                RaisePropertyChanged();
            }
        }

        public bool IsEnd
        {
            get { return m_isEnd; }
            set
            {
                m_isEnd = value;
                RaisePropertyChanged();
            }
        }
        
        public void UpdatePlayerOrder()
        {
            PlayerRanking = new ObservableCollection<ProgressViewModel>(PlayerRanking.OrderBy(player => player, new PlayerProgressComparer()));
        }
    }
}