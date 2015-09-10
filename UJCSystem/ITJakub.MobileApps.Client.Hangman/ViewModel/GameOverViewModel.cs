using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Hangman.ViewModel.Comparer;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class GameOverViewModel : ViewModelBase
    {
        private bool m_win;
        private bool m_loss;
        private bool m_showTable;
        private ProgressInfoViewModel m_myProgressInfo;
        private ObservableCollection<ProgressInfoViewModel> m_playerRanking;

        public GameOverViewModel()
        {
            PlayerRanking = new ObservableCollection<ProgressInfoViewModel>();
            ShowTable = true;
        }

        public ObservableCollection<ProgressInfoViewModel> PlayerRanking
        {
            get { return m_playerRanking; }
            private set
            {
                m_playerRanking = value;
                RaisePropertyChanged();
            }
        }

        public bool Win
        {
            get { return m_win; }
            set
            {
                m_win = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => GameOver);
            }
        }

        public bool Loss
        {
            get { return m_loss; }
            set
            {
                m_loss = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => GameOver);
            }
        }

        public bool ShowTable
        {
            get { return m_showTable; }
            set
            {
                m_showTable = value;
                RaisePropertyChanged();
            }
        }

        public bool GameOver
        {
            get { return Loss || Win; }
        }

        public void UpdateMyProgress(ProgressInfoViewModel progressInfo)
        {
            if (m_myProgressInfo == null)
            {
                m_myProgressInfo = new ProgressInfoViewModel
                {
                    UserInfo = progressInfo.UserInfo,
                    FirstUpdateTime = progressInfo.Time
                };
                AddPlayerViewModel(m_myProgressInfo);
            }
            m_myProgressInfo.HangmanCount = progressInfo.HangmanCount;
            m_myProgressInfo.LivesRemain = progressInfo.LivesRemain;
            m_myProgressInfo.GuessedWordCount = progressInfo.GuessedWordCount;
            m_myProgressInfo.LetterCount = progressInfo.LetterCount;
            m_myProgressInfo.Win = progressInfo.Win;
            m_myProgressInfo.Time = progressInfo.Time;
        }

        public void AddPlayerViewModel(ProgressInfoViewModel progressInfo)
        {
            PlayerRanking.Add(progressInfo);
        }

        public void UpdatePlayerPositions()
        {
            PlayerRanking = new ObservableCollection<ProgressInfoViewModel>(PlayerRanking.OrderBy(model => model, new PlayerProgressComparer()));
        }
    }
}