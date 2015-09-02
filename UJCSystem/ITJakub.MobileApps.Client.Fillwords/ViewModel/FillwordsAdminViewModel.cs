using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ITJakub.MobileApps.Client.Fillwords.DataService;
using ITJakub.MobileApps.Client.Fillwords.ViewModel.Comparer;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class FillwordsAdminViewModel : AdminBaseViewModel
    {
        private readonly FillwordsDataService m_dataService;
        private readonly Dictionary<long, UserResultViewModel> m_results;
        private ObservableCollection<UserResultViewModel> m_resultList;
        
        public FillwordsAdminViewModel(FillwordsDataService dataService)
        {
            m_dataService = dataService;
            m_results = new Dictionary<long, UserResultViewModel>();
            ResultList = new ObservableCollection<UserResultViewModel>();
        }

        public ObservableCollection<UserResultViewModel> ResultList
        {
            get { return m_resultList; }
            set
            {
                m_resultList = value;
                RaisePropertyChanged();
            }
        }

        public override void SetTask(string data)
        {
            m_dataService.SetTaskAndGetData(data, task => {});
        }

        public override void InitializeCommunication()
        {
            m_dataService.ResetLastRequestTime();
            m_dataService.StartPollingResults((newResults, exception) =>
            {
                if (exception != null)
                {
                    m_dataService.ErrorService.ShowConnectionWarning();
                    return;
                }

                foreach (var userResultViewModel in newResults)
                {
                    if (m_results.ContainsKey(userResultViewModel.UserInfo.Id))
                    {
                        var originUserResult = m_results[userResultViewModel.UserInfo.Id];
                        originUserResult.UserInfo = userResultViewModel.UserInfo;
                        originUserResult.Answers = userResultViewModel.Answers;
                        originUserResult.CorrectAnswers = userResultViewModel.CorrectAnswers;
                        originUserResult.TotalAnswers = userResultViewModel.TotalAnswers;
                        originUserResult.IsTaskSubmited = userResultViewModel.IsTaskSubmited;
                    }
                    else
                    {
                        m_results.Add(userResultViewModel.UserInfo.Id, userResultViewModel);
                        ResultList.Add(userResultViewModel);
                    }
                }

                if (newResults.Count > 0)
                    SortResultList();
            });
        }

        public override void UpdateGroupMembers(IEnumerable<UserInfo> members)
        {
            foreach (var member in members)
            {
                if (m_results.ContainsKey(member.Id))
                    continue;

                var emptyUserResult = new UserResultViewModel
                {
                    UserInfo = member
                };
                m_results.Add(member.Id, emptyUserResult);
                ResultList.Add(emptyUserResult);
            }

            SortResultList();
        }

        private void SortResultList()
        {
            ResultList = new ObservableCollection<UserResultViewModel>(ResultList.OrderBy(x => x.UserInfo, new UserInfoNameComparer()));
        }
    }
}