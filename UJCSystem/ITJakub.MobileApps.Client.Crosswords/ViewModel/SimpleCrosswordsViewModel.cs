using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Crosswords.DataService;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class SimpleCrosswordsViewModel : ViewModelBase
    {
        private readonly ICrosswordsDataService m_dataService;
        private ObservableCollection<CrosswordRowViewModel> m_crossword;
        private ObservableCollection<CrosswordRowViewModel> m_myCrossword;
        private ObservableCollection<CrosswordRowViewModel> m_correctCrossword;
        private bool m_firstUpdate;
        private bool m_showCorrectResult;
        
        public SimpleCrosswordsViewModel(ICrosswordsDataService dataService)
        {
            m_dataService = dataService;
            m_firstUpdate = true;
            Crossword = new ObservableCollection<CrosswordRowViewModel>();
            OpponentProgress = new ObservableCollection<ProgressViewModel>();
            ShowResultsCommand = new RelayCommand(() => PlayerRankingViewModel.IsResultTableVisible = true);
        }
        
        public Action GoBack { get; set; }

        public ObservableCollection<CrosswordRowViewModel> Crossword
        {
            get { return m_crossword; }
            set
            {
                m_crossword = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowCorrectResult
        {
            get { return m_showCorrectResult; }
            set
            {
                m_showCorrectResult = value;
                RaisePropertyChanged();

                Crossword = ShowCorrectResult ? m_correctCrossword : m_myCrossword;
            }
        }

        public ObservableCollection<ProgressViewModel> OpponentProgress { get; set; }

        public PlayerRankingViewModel PlayerRankingViewModel { get; set; }

        public RelayCommand ShowResultsCommand { get; private set; }
        
        private void SetSubmitCallbacks(IEnumerable<CrosswordRowViewModel> crosswordRows)
        {
            foreach (CrosswordRowViewModel rowViewModel in crosswordRows)
            {
                rowViewModel.FillWordAction = SubmitAction;
            }
        }

        private void SubmitAction(CrosswordRowViewModel rowViewModel)
        {
            m_dataService.FillWord(rowViewModel.RowIndex, rowViewModel.Word, (gameInfo, exception) =>
            {
                if (exception != null)
                {
                    m_dataService.ErrorService.ShowConnectionError(GoBack);
                    return;
                }

                rowViewModel.IsCorrect = gameInfo.WordFilledCorrectly;
                SetIsEnd(gameInfo.Win);
            });
        }

        public void UpdateProgress(IList<ProgressUpdateViewModel> list)
        {
            foreach (var progressUpdate in list)
            {
                UpdatePlayerProgress(progressUpdate);
                
                if (progressUpdate.UserInfo.IsMe)
                {
                    // Load my progress history (only first load)
                    if (!m_firstUpdate)
                        continue;

                    var rowViewModel = Crossword.First(row => row.RowIndex == progressUpdate.RowIndex);
                    rowViewModel.UpdateWord(progressUpdate.FilledWord);
                    rowViewModel.IsCorrect = progressUpdate.IsCorrect;
                }
            }
            m_firstUpdate = false;
            if (list.Count > 0)
                PlayerRankingViewModel.UpdatePlayerOrder();
        }

        private void UpdatePlayerProgress(ProgressUpdateViewModel progressUpdate)
        {
            var viewModel = PlayerRankingViewModel.PlayerRanking.SingleOrDefault(model => model.UserInfo.Id == progressUpdate.UserInfo.Id);
            if (viewModel == null)
            {
                var rowProgressViewModels = Crossword.Select(model => model.Cells != null
                    ? new RowProgressViewModel(model.RowIndex, model.Cells.Count, model.StartPosition, model.AnswerPosition)
                    : new RowProgressViewModel());

                viewModel = new ProgressViewModel(progressUpdate.UserInfo, progressUpdate.Time)
                {
                    Rows = new ObservableCollection<RowProgressViewModel>(rowProgressViewModels)
                };

                PlayerRankingViewModel.PlayerRanking.Add(viewModel);

                if (!progressUpdate.UserInfo.IsMe)
                    OpponentProgress.Add(viewModel);
            }
            var rowViewModel = viewModel.Rows.First(row => row.RowIndex == progressUpdate.RowIndex);
            rowViewModel.FilledLength = progressUpdate.FilledWord.Length;
            rowViewModel.IsCorrect = progressUpdate.IsCorrect;

            viewModel.CorrectAnswers = viewModel.Rows.Count(x => x.IsCorrect);
            viewModel.Win = viewModel.Rows.Where(x => x.Cells != null).All(x => x.IsCorrect);
            viewModel.UpdateTime(progressUpdate.Time);
        }

        private void SetIsEnd(bool value)
        {
            if (value && m_correctCrossword == null)
            {
                m_dataService.GetConfiguration(rowList =>
                {
                    m_correctCrossword = rowList;
                }, true);
            }

            PlayerRankingViewModel.IsEnd = value;
        }

        public void SetWin(bool isWin)
        {
            if (PlayerRankingViewModel != null && isWin)
                SetIsEnd(true);
        }

        public void StopAndShowResults()
        {
            SetIsEnd(true);
        }

        public void SetCrossword(ObservableCollection<CrosswordRowViewModel> crosswordRowViewModels)
        {
            m_myCrossword = crosswordRowViewModels;
            Crossword = crosswordRowViewModels;
            SetSubmitCallbacks(m_crossword);
            PlayerRankingViewModel = new PlayerRankingViewModel();
        }
    }
}