using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Crosswords.ViewModel.Comparer;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class PlayerRankingViewModel : ViewModelBase
    {
        private bool m_isEnd;
        private ObservableCollection<ProgressViewModel> m_playerRanking;
        private bool m_isResultTableVisible;

        public PlayerRankingViewModel()
        {
            PlayerRanking = new ObservableCollection<ProgressViewModel>();
            CloseResultsCommand = new RelayCommand(() => IsResultTableVisible = false);
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

                if (m_isEnd)
                    IsResultTableVisible = true;
            }
        }

        public bool IsResultTableVisible
        {
            get { return m_isResultTableVisible; }
            set
            {
                m_isResultTableVisible = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand CloseResultsCommand { get; private set; }

        public void UpdatePlayerOrder()
        {
            PlayerRanking = new ObservableCollection<ProgressViewModel>(PlayerRanking.OrderBy(player => player, new PlayerProgressComparer()));
        }
    }
}