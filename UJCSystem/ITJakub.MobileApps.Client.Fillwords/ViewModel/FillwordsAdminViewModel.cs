using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Fillwords.DataService;
using ITJakub.MobileApps.Client.Fillwords.ViewModel.Comparer;
using ITJakub.MobileApps.Client.Fillwords.ViewModel.Enum;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class FillwordsAdminViewModel : AdminBaseViewModel
    {
        private readonly FillwordsDataService m_dataService;
        private readonly Dictionary<long, UserResultViewModel> m_results;
        private ObservableCollection<UserResultViewModel> m_resultList;
        private UserResultViewModel m_selectedUser;
        private string m_taskDocumentRtf;
        private ObservableCollection<OptionsViewModel> m_taskOptionsList;
        private bool m_isUserNotAnsweredError;

        public FillwordsAdminViewModel(FillwordsDataService dataService)
        {
            m_dataService = dataService;
            m_results = new Dictionary<long, UserResultViewModel>();
            ResultList = new ObservableCollection<UserResultViewModel>();

            ShowCorrectAnswersCommand = new RelayCommand(ShowCorrectAnswers);
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

        public UserResultViewModel SelectedUser
        {
            get { return m_selectedUser; }
            set
            {
                m_selectedUser = value;
                RaisePropertyChanged();
                ShowAnswersForUser(m_selectedUser);
            }
        }
        
        public string TaskDocumentRtf
        {
            get { return m_taskDocumentRtf; }
            set
            {
                m_taskDocumentRtf = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<OptionsViewModel> TaskOptionsList
        {
            get { return m_taskOptionsList; }
            set
            {
                m_taskOptionsList = value;
                RaisePropertyChanged();
            }
        }
        
        public bool IsUserNotAnsweredError
        {
            get { return m_isUserNotAnsweredError; }
            set
            {
                m_isUserNotAnsweredError = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ShowCorrectAnswersCommand { get; private set; }

        public override void SetTask(string data)
        {
            m_dataService.SetTaskAndGetData(data, task =>
            {
                var newOptionsList = new ObservableCollection<OptionsViewModel>();
                foreach (var optionsViewModel in task.Options)
                {
                    var newOptionsViewModel = new OptionsViewModel
                    {
                        AnswerState = AnswerState.NoAnswer,
                        CorrectAnswer = optionsViewModel.CorrectAnswer,
                        List = optionsViewModel.List,
                        SelectedAnswer = optionsViewModel.CorrectAnswer,
                        WordPosition = optionsViewModel.WordPosition
                    };
                    newOptionsList.Add(newOptionsViewModel);
                }

                TaskDocumentRtf = task.DocumentRtf;
                TaskOptionsList = newOptionsList;
                SelectedUser = null;
            });
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
                m_dataService.ErrorService.HideWarning();
                SetDataLoaded();

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

        private void ShowAnswersForUser(UserResultViewModel currentUserResult)
        {
            if (currentUserResult == null)
                return;

            if (!currentUserResult.IsTaskSubmited)
            {
                ShowUnknownAnswers();
                return;
            }

            IsUserNotAnsweredError = false;
            for (int i = 0; i < TaskOptionsList.Count; i++)
            {
                var optionsViewModel = TaskOptionsList[i];
                var resultUserAnswer = currentUserResult.Answers[i];

                optionsViewModel.UpdateSelectedAnswer(resultUserAnswer.Answer);
                optionsViewModel.AnswerState = resultUserAnswer.IsCorrect
                    ? AnswerState.Correct
                    : AnswerState.Incorrect;
            }
        }

        private void ShowUnknownAnswers()
        {
            IsUserNotAnsweredError = true;
            foreach (var optionsViewModel in TaskOptionsList)
            {
                optionsViewModel.UpdateSelectedAnswer(null);
                optionsViewModel.AnswerState = AnswerState.NoAnswer;
            }
        }

        private void ShowCorrectAnswers()
        {
            SelectedUser = null;
            IsUserNotAnsweredError = false;

            foreach (var optionsViewModel in TaskOptionsList)
            {
                optionsViewModel.UpdateSelectedAnswer(optionsViewModel.CorrectAnswer);
                optionsViewModel.AnswerState = AnswerState.NoAnswer;
            }
        }

        private void SortResultList()
        {
            ResultList = new ObservableCollection<UserResultViewModel>(ResultList.OrderBy(x => x.UserInfo, new UserInfoNameComparer()));
        }
    }
}