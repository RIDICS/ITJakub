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