using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ITJakub.MobileApps.Client.Crosswords.DataService;
using ITJakub.MobileApps.Client.Crosswords.ViewModel.Comparer;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class CrosswordsAdminViewModel : AdminBaseViewModel
    {
        private readonly ICrosswordsDataService m_dataService;
        private readonly Dictionary<long, ProgressViewModel> m_memberProgress;
        private ObservableCollection<ProgressViewModel> m_playerRanking;
        private ObservableCollection<CrosswordRowViewModel> m_rowListPattern;

        public CrosswordsAdminViewModel(ICrosswordsDataService dataService)
        {
            m_dataService = dataService;
            m_memberProgress = new Dictionary<long, ProgressViewModel>();
            PlayerRanking = new ObservableCollection<ProgressViewModel>();
        }

        public override void SetTask(string data)
        {
            m_dataService.SetTaskAndGetConfiguration(data, rowList =>
            {
                m_rowListPattern = rowList;
            }, true);
        }

        public override void InitializeCommunication()
        {
            m_dataService.ResetLastRequestTime();
            m_dataService.StartPollingProgress((progressUpdateList, exception) =>
            {
                if (exception != null)
                {
                    m_dataService.ErrorService.ShowConnectionWarning();
                    return;
                }
                m_dataService.ErrorService.HideWarning();

                UpdateProgress(progressUpdateList);
            });
        }

        private void UpdateProgress(List<ProgressUpdateViewModel> progressUpdateList)
        {
            foreach (var progressUpdate in progressUpdateList)
            {
                ProgressViewModel viewModel;
                
                if (!m_memberProgress.ContainsKey(progressUpdate.UserInfo.Id))
                {
                    viewModel = CreateProgressViewModel(progressUpdate.UserInfo, progressUpdate.Time);

                    m_memberProgress.Add(progressUpdate.UserInfo.Id, viewModel);
                    PlayerRanking.Add(viewModel);
                }
                else
                {
                    viewModel = m_memberProgress[progressUpdate.UserInfo.Id];
                }

                var rowViewModel = viewModel.Rows.First(row => row.RowIndex == progressUpdate.RowIndex);
                rowViewModel.FilledLength = progressUpdate.FilledWord.Length;
                rowViewModel.IsCorrect = progressUpdate.IsCorrect;

                viewModel.CorrectAnswers = viewModel.Rows.Count(x => x.IsCorrect);
                viewModel.Win = viewModel.Rows.Where(x => x.Cells != null).All(x => x.IsCorrect);
                viewModel.UpdateTime(progressUpdate.Time);
            }

            if (progressUpdateList.Count > 0)
            {
                PlayerRanking = new ObservableCollection<ProgressViewModel>(PlayerRanking.OrderBy(x => x, new PlayerProgressComparer()));
            }
        }

        public override void UpdateGroupMembers(IEnumerable<UserInfo> members)
        {
            foreach (var memberInfo in members.Where(x => !m_memberProgress.ContainsKey(x.Id)))
            {
                var newProgressInfo = CreateProgressViewModel(memberInfo);

                m_memberProgress.Add(memberInfo.Id, newProgressInfo);
                PlayerRanking.Add(newProgressInfo);
            }

            PlayerRanking = new ObservableCollection<ProgressViewModel>(PlayerRanking.OrderBy(x => x, new PlayerProgressComparer()));
        }

        private ProgressViewModel CreateProgressViewModel(UserInfo userInfo, DateTime? firstTime = null)
        {
            var rowProgressViewModels = m_rowListPattern.Select(model => model.Cells != null
                ? new RowProgressViewModel(model.RowIndex, model.Cells.Count, model.StartPosition, model.AnswerPosition)
                : new RowProgressViewModel());

            var viewModel = new ProgressViewModel(userInfo, firstTime)
            {
                UserInfo = userInfo,
                Rows = new ObservableCollection<RowProgressViewModel>(rowProgressViewModels)
            };

            return viewModel;
        }

        public ObservableCollection<ProgressViewModel> PlayerRanking
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