using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Crosswords.ViewModel.Comparer;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class PlayerRankingViewModel : ViewModelBase
    {
        private readonly int m_crosswordRowCount;
        private bool m_isEnd;
        private ObservableCollection<ProgressViewModel> m_playerRanking;
        private HashSet<long> m_playersInList;

        public PlayerRankingViewModel(int crosswordRowCount)
        {
            m_crosswordRowCount = crosswordRowCount;
            m_playersInList = new HashSet<long>();
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

        //public void UpdatePlayerProgressList(ProgressViewModel progressViewModel)
        //{
        //    if (m_playersInList.Contains(progressViewModel.UserInfo.Id))
        //        return;

        //    m_playersInList.Add(progressViewModel.UserInfo.Id);
        //    PlayerRanking.Add(progressViewModel);
        //}

        //public void UpdatePlayerRank(ProgressUpdateViewModel progressUpdate)
        //{
        //    var playerRank = PlayerRanking.SingleOrDefault(model => model.UserInfo.Id == progressUpdate.UserInfo.Id);

        //    if (playerRank == null)
        //    {
        //        playerRank = new PlayerRankViewModel(m_crosswordRowCount, progressUpdate.UserInfo, progressUpdate.Time);
        //        PlayerRanking.Add(playerRank);
        //    }

        //    playerRank.UpdateRowInfo(progressUpdate.RowIndex, progressUpdate.IsCorrect);
        //    playerRank.UpdateTime(progressUpdate.Time);
        //}

        public void UpdatePlayerOrder()
        {
            PlayerRanking = new ObservableCollection<ProgressViewModel>(PlayerRanking.OrderBy(player => player, new PlayerProgressComparer()));
        }
    }

    //public class PlayerRankViewModel2 : ViewModelBase
    //{
    //    private readonly DateTime m_firstTime;
    //    private readonly bool[] m_correctAnswers;
    //    private bool m_win;
    //    private int m_letterCount;

    //    public PlayerRankViewModel(int crosswordRowCount, UserInfo userInfo, DateTime firstTime)
    //    {
    //        m_firstTime = firstTime;
    //        UserInfo = userInfo;
    //        m_correctAnswers = new bool[crosswordRowCount];
    //    }

    //    public UserInfo UserInfo { get; private set; }

    //    public TimeSpan GameTime { get; private set; }

    //    public bool Win
    //    {
    //        get { return m_win; }
    //        private set
    //        {
    //            m_win = value;
    //            RaisePropertyChanged();
    //        }
    //    }

    //    public int LetterCount
    //    {
    //        get { return m_letterCount; }
    //        private set
    //        {
    //            m_letterCount = value;
    //            RaisePropertyChanged();
    //        }
    //    }

    //    public void UpdateRowInfo(int row, bool isCorrect)
    //    {
    //        m_correctAnswers[row] = isCorrect;
    //        LetterCount = m_correctAnswers.Count(b => b);
    //        Win = m_correctAnswers.All(correctAnswer => correctAnswer);
    //    }

    //    public void UpdateTime(DateTime time)
    //    {
    //        GameTime = time - m_firstTime;
    //    }
    //}
}