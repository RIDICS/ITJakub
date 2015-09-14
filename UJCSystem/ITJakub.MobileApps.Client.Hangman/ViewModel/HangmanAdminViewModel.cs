using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ITJakub.MobileApps.Client.Hangman.DataService;
using ITJakub.MobileApps.Client.Hangman.ViewModel.Comparer;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class HangmanAdminViewModel : AdminBaseViewModel
    {
        private readonly IHangmanDataService m_dataService;
        private readonly Dictionary<long, ProgressInfoViewModel> m_memberProgress;
        private ObservableCollection<ProgressInfoViewModel> m_playerRanking;
        
        public HangmanAdminViewModel(IHangmanDataService dataService)
        {
            m_dataService = dataService;
            m_memberProgress = new Dictionary<long, ProgressInfoViewModel>();
            PlayerRanking = new ObservableCollection<ProgressInfoViewModel>();
        }

        public override void SetTask(string data)
        {
            
        }

        public override void InitializeCommunication()
        {
            m_dataService.StartPollingProgress((progressList, exception) =>
            {
                if (exception != null)
                {
                    m_dataService.ErrorService.ShowConnectionWarning();
                    return;
                }

                ProcessUserProgress(progressList);
            });
        }
        
        public override void UpdateGroupMembers(IEnumerable<UserInfo> members)
        {
            foreach (var memberInfo in members.Where(x => !m_memberProgress.ContainsKey(x.Id)))
            {
                var newProgressInfo = new ProgressInfoViewModel
                {
                    UserInfo = memberInfo
                };
                m_memberProgress.Add(memberInfo.Id, newProgressInfo);
                PlayerRanking.Add(newProgressInfo);
            }

            SortPlayers();
        }

        private void ProcessUserProgress(ObservableCollection<ProgressInfoViewModel> progressList)
        {
            foreach (var progressInfo in progressList)
            {
                ProgressInfoViewModel viewModel;
                if (m_memberProgress.TryGetValue(progressInfo.UserInfo.Id, out viewModel))
                {
                    viewModel.HangmanCount = progressInfo.HangmanCount;
                    viewModel.LivesRemain = progressInfo.LivesRemain;
                    viewModel.GuessedWordCount = progressInfo.GuessedWordCount;
                    viewModel.LetterCount = progressInfo.LetterCount;
                    viewModel.Win = progressInfo.Win;
                    viewModel.Time = progressInfo.Time;
                }
                else
                {
                    progressInfo.FirstUpdateTime = progressInfo.Time;
                    PlayerRanking.Add(progressInfo);
                    m_memberProgress.Add(progressInfo.UserInfo.Id, progressInfo);
                }
            }
            if (progressList.Count > 0)
            {
                SortPlayers();
            }
        }

        private void SortPlayers()
        {
            PlayerRanking = new ObservableCollection<ProgressInfoViewModel>(PlayerRanking.OrderBy(x => x, new PlayerProgressComparer()));
        }

        public ObservableCollection<ProgressInfoViewModel> PlayerRanking
        {
            get { return m_playerRanking; }
            set
            {
                m_playerRanking = value;
                RaisePropertyChanged();
            }
        }
    }
}